using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Web;
using Newtonsoft.Json;
using ErrorOr;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Commerce.API.Client.Base;

public class BaseClient : IBaseClient
{
    private readonly string _baseUri;
    private readonly HttpClient _client;

    public BaseClient(HttpClient client, string baseUri)
    {
        _client = client;
        _baseUri = baseUri;
    }
    
    public async Task<ErrorOr<byte[]>> Get(Uri uri, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.GetAsync(uri, ct);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsByteArrayAsync(ct);
            }
            return await ErrorContent<byte[]>(response, ct);
        }
        catch (Exception ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }

    public async Task<ErrorOr<T>> Get<T>(Uri uri, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.GetAsync(uri, ct);
            return await ReadContentAs<T>(response, ct);
        }
        catch (Exception ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }

    public async Task<ErrorOr<T>> PostAsJson<T, U>(Uri uri, U data, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.PostAsJsonAsync(uri, data, ct);
            return await ReadContentAs<T>(response, ct);
        }
        catch (Exception ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }

    public async Task<ErrorOr<T>> PutAsJson<T, U>(Uri uri, U data, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.PutAsJsonAsync(uri, data, ct);
            return await ReadContentAs<T>(response, ct);
        }
        catch (Exception ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }
    
    public async Task<ErrorOr<T>> PatchAsJson<T, U>(Uri uri, U data, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.PatchAsJsonAsync(uri, data, ct);
            return await ReadContentAs<T>(response, ct);
        }
        catch (Exception ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }

    public async Task<ErrorOr<T>> Patch<T>(Uri uri, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.PatchAsync(uri.OriginalString, null, ct);
            return await ReadContentAs<T>(response, ct);
        }
        catch (Exception ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }

    public async Task<ErrorOr<T>> Delete<T>(Uri uri, CancellationToken ct = default)
    {
        try
        {
            var response = await _client.DeleteAsync(uri, ct);
            return await ReadContentAs<T>(response, ct);
        }
        catch (Exception ex)
        {
            return Error.Failure(description: ex.Message);
        }
    }

    public async Task<ErrorOr<T>> ReadContentAs<T>(HttpResponseMessage response, CancellationToken ct = default)
    {
        if (response.IsSuccessStatusCode)
        {
            var dataAsString = await response.Content.ReadAsStringAsync(ct);
            return JsonConvert.DeserializeObject<T>(dataAsString)!;
        }

        return await ErrorContent<T>(response, ct);
    }

    public async Task<ErrorOr<T>> ErrorContent<T>(HttpResponseMessage response, CancellationToken ct = default)
    {
        var statusCode = (int)response.StatusCode;
        if (statusCode == StatusCodes.Status304NotModified)
        {
            return Error.Custom(StatusCodes.Status304NotModified, 
                nameof(StatusCodes.Status304NotModified), 
                "The requested resource has not been modified since the last time it was loaded.");
        }
        
        string? description;
        try
        {
            var problemAsString = await response.Content.ReadAsStringAsync(ct);
            try
            {
                var problem = JsonConvert.DeserializeObject<ProblemDetails>(problemAsString)!;
                if (problem.Status == StatusCodes.Status400BadRequest)
                {
                    try
                    {
                        var validationProblem = JsonConvert.DeserializeObject<HttpValidationProblemDetails>(problemAsString)!;
                        var errors = validationProblem.Errors;
                        return errors.Select(kvp =>
                            Error.Validation(kvp.Key, kvp.Value.First())).ToList();
                    }
                    catch
                    {
                        // ignore
                    }
                }
                description = problem.Detail ?? problem.Title ?? string.Empty;
            }
            catch
            {
                description = problemAsString;
            }
        }
        catch
        {
            description = response.ReasonPhrase ?? string.Empty;
        }
        
        switch (statusCode)
        {
            case StatusCodes.Status400BadRequest:
                try
                {
                    var errors = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(description);
                    return errors!.Select(kvp => 
                        Error.Validation(kvp.Key, kvp.Value.First())).ToList();
                }
                catch
                {
                    return Error.Validation(statusCode.ToString(), description);
                }
            case StatusCodes.Status401Unauthorized:
                try
                {
                    var errors = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(description);
                    return errors!.Select(kvp => 
                        Error.Unauthorized(kvp.Key, kvp.Value.First())).ToList();
                }
                catch
                {
                    return Error.Unauthorized(statusCode.ToString(), description);
                }
            case StatusCodes.Status403Forbidden:
                try
                {
                    var errors = JsonConvert.DeserializeObject<Dictionary<string, string[]>>(description);
                    return errors!.Select(kvp => 
                        Error.Forbidden(kvp.Key, kvp.Value.First())).ToList();
                }
                catch
                {
                    return Error.Forbidden(statusCode.ToString(), description);
                }
            case StatusCodes.Status404NotFound:
                return Error.NotFound(statusCode.ToString(), description);
            case StatusCodes.Status409Conflict:
            case StatusCodes.Status412PreconditionFailed:
            case StatusCodes.Status499ClientClosedRequest:
                return Error.Conflict(statusCode.ToString(), description);
            default:
                return Error.Failure(statusCode.ToString(), description);
        }
    }

    public Uri BuildUri(string path, Dictionary<string, string>? queries = default)
    {
        var uri = new UriBuilder(_baseUri)
        {
            Path = path
        }.Uri;

        if (queries?.Count > 0)
        {
            foreach (var kvp in queries)
            {
                uri = AddQuery(uri, kvp.Key, kvp.Value);
            }
        }
        return uri;
    }

    private Uri AddQuery(Uri uri, string name, string value)
    {
        var httpValueCollection = HttpUtility.ParseQueryString(uri.Query);

        httpValueCollection.Remove(name);
        httpValueCollection.Add(name, value);

        var ub = new UriBuilder(uri)
        {
            Query = httpValueCollection.ToString()
        };

        return ub.Uri;
    }
}