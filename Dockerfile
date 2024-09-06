FROM mcr.microsoft.com/dotnet/aspnet:8.0-jammy AS base
WORKDIR /app
EXPOSE 5002

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /project
COPY ["/src/Commerce.API/Commerce.API.csproj", "/src/Commerce.API/"]
COPY ["/src/Commerce.API.Client/Commerce.API.Client.csproj", "/src/Commerce.API.Client/"]
COPY ["/src/Commerce.API.Contract/Commerce.API.Contract.csproj", "/src/Commerce.API.Contract/"]
COPY ["/src/Commerce.Application/Commerce.Application.csproj", "/src/Commerce.Application/"]
COPY ["/src/Commerce.Domain/Commerce.Domain.csproj", "/src/Commerce.Domain/"]
COPY ["/src/Commerce.Infrastructure/Commerce.Infrastructure.csproj", "/src/Commerce.Infrastructure/"]
RUN dotnet restore "/src/Commerce.API/Commerce.API.csproj"
COPY . .
WORKDIR "/project/src/Commerce.API"
RUN dotnet build "Commerce.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Commerce.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
 
ENTRYPOINT ["dotnet", "Commerce.API.dll"]