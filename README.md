# Commerce API

Welcome to the Commerce API repository! This project provides an API to manage customer data, including functionalities to paginate through customer lists. It is built using .NET Minimal APIs and follows best practices for performance and scalability.

## Features

**1. .NET Minimal APIs:**
- Built with .NET Minimal APIs to ensure high efficiency and streamlined performance.

**2. Domain-Driven Design (DDD) Principles:** 
- Utilizes loosely coupled Domain-Driven Design principles to promote a modular and maintainable codebase.

**3. MartenDB Integration:**
- Leverages MartenDB for robust document storage and advanced event sourcing capabilities.

**4. Advanced Query Support:**
- Supports comprehensive filtering, keyset, and offset pagination for flexible data retrieval.
- Utilizes compiled queries for improved query performance and efficiency.

**5. Partial Updates:**
- Enables partial updates via the PATCH method, allowing for more efficient data modifications.

**6. Strongly Typed IDs and Smart Enums:**
- Implements strongly typed IDs and smart enums to enhance type safety and reduce errors associated with incorrect ID usage.

**7. Authorization Mechanisms:**
- Supports both API Key and OAuth 2.0 access token authorization for secure API access.

**8. Standardized HTTP Responses:**
- Provides standardized HTTP status codes, detailed error handling, and validation problem details to enhance API reliability and client experience.

**9. Optimistic Concurrency Control:**
- Utilizes ETag headers to manage optimistic concurrency control effectively.

**10. Efficient Data Transfer:**
- Supports If-None-Match headers to minimize redundant data transfers through client-side caching.

**11. Performance and Scalability:**
- Enhances performance and scalability by utilizing Redis for caching and RabbitMQ for messaging.

**12. Comprehensive Testing:**
- Contains extensive xUnit tests covering both API endpoints and database operations to ensure code quality and reliability.

## Installation

### Prerequisites

The API requires the following services to be running:

- PostgreSQL on port 5432
- Redis on port 6379
- RabbitMQ on port 5672

### Steps

1. Clone the repository:
    ```bash
    git clone https://github.com/amir-ae/commerce-api.git
    cd commerce-api
    ```

2. Restore the dependencies:
    ```bash
    dotnet restore
    ```

3. Build the project:
    ```bash
    dotnet build
    ```

4. Run the application:
    ```bash
    cd src/Commerce.API
    dotnet run
    ```

## Usage

After running the application, the API will be accessible at `https://localhost:44302`.

## API Endpoints

The Commerce API provides several endpoints to manage customer data. Below are the details of example endpoints:

### Customers Endpoints

#### GET `/api/v1/customers`

Fetches a paginated list of customers.

**Parameters:**
- `pageSize` (int, optional): Number of customers per page. Default is 10.
- `pageNumber` (int, optional): Page number to fetch. Default is 1.
- `nextPage` (bool, optional): Indicates if the next page should be fetched.
- `keyId` (string, optional): Customer key ID to enable keyset pagination.
- `centreId` (Guid, optional): Filter by centre ID.

**Responses:**
- `200 OK`: Returns a paginated list of customers.
- `304 Not Modified`
- `400 Bad Request`: Validation error.
- `401 Unauthorized`
- `403 Forbidden`
- `499 Client Closed Request`
- `500 Internal Server Error`

**Example Request:**
```bash
curl -X GET "https://localhost:44302/api/v1/customers?pageSize=10&pageNumber=1"
```

**Example Response:**
```bash
{
  "pageNumber": 1,
  "pageSize": 10,
  "total": 1,
  "data": [
    {
      "id": "98acc088-4f93-43e1-b5e4-d2ce6749aed8",
      "firstName": "Антон",
      "middleName": null,
      "lastName": "Иванов",
      "fullName": "Иванов Антон",
      "phoneNumber": {
        "value": "(454) 556 76 77",
        "countryId": "RU",
        "countryCode": "+7",
        "description": null,
        "fullNumber": "+7 (454) 556 76 77"
      },
      "city": {
        "cityId": 698,
        "name": null,
        "oblast": null,
        "postalCode": null,
        "phoneCode": null
      },
      "address": "ул. Светлая, д. 21, кв. 5",
      "role": "Владелец",
      "productIds": [
        "110AS30924PL12TC000733"
      ],
      "orders": [
        {
          "id": "fa97731d-539b-4878-b9c9-b4b0465d9851",
          "centreId": "77152850-7ea2-4a3f-abc4-6888137e4e3a"
        }
      ],
      "products": null,
      "createdAt": "2024-07-03T01:57:53.0643226+03:00",
      "createdBy": "7b478701-8c45-422e-a7ec-898141860906",
      "lastModifiedAt": null,
      "lastModifiedBy": null,
      "version": 1,
      "isActive": true,
      "isDeleted": false
    }
  ]
}
```

#### GET `/api/v1/customers/{id}`

Fetches a single customer by ID.

**Parameters:**
- `id` (string, required): The ID of the customer to fetch.

**Conditional Requests with If-None-Match:**
- The `If-None-Match` header is used to handle conditional requests. This is particularly useful for reducing unnecessary data transfer and improving efficiency.
- How It Works: When you make a request to fetch a resource, you can include the `If-None-Match` header with the ETag value you previously received. The server compares this value with the current version of the resource. If they match, indicating that the resource has not changed, the server returns a `304 Not Modified` response, indicating that the cached version can be used. This is applicable even when fetching data by page.

**Responses:**
- `200 OK`: Returns the customer detail.
- `304 Not Modified`
- `400 Bad Request`: Validation error.
- `401 Unauthorized`
- `403 Forbidden`
- `404 NotFound`: Customer not found.
- `499 Client Closed Request`
- `500 Internal Server Error`

**Example Request:**
```bash
curl -X GET "https://localhost:44302/api/v1/customers/98acc088-4f93-43e1-b5e4-d2ce6749aed8"
```

**Example Response:**
```bash
{
  "id": "98acc088-4f93-43e1-b5e4-d2ce6749aed8",
  "firstName": "Антон",
  "middleName": null,
  "lastName": "Иванов",
  "fullName": "Иванов Антон",
  "phoneNumber": {
    "value": "(454) 556 76 77",
    "countryId": "RU",
    "countryCode": "+7",
    "description": null,
    "fullNumber": "+7 (454) 556 76 77"
  },
  "city": {
    "cityId": 698,
    "name": null,
    "oblast": null,
    "postalCode": null,
    "phoneCode": null
  },
  "address": "ул. Светлая, д. 21, кв. 5",
  "role": "Владелец",
  "productIds": [
    "110AS30924PL12TC000733"
  ],
  "orders": [
    {
      "id": "fa97731d-539b-4878-b9c9-b4b0465d9851",
      "centreId": "77152850-7ea2-4a3f-abc4-6888137e4e3a"
    }
  ],
  "products": null,
  "createdAt": "2024-07-03T01:57:53.0643226+03:00",
  "createdBy": "7b478701-8c45-422e-a7ec-898141860906",
  "lastModifiedAt": null,
  "lastModifiedBy": null,
  "version": 1,
  "isActive": true,
  "isDeleted": false
}
```

#### POST `/api/v1/customers`

Creates a new customer.

**Request Body:**
- `createBy` (Guid, required): The ID of the user making the request.
- `firstName` (string, required): The first name of the customer.
- `middleName` (string, optional): The middle name of the customer.
- `lastName` (string, required): The last name of the customer.
- `cityId` (int, required): The ID of the city where the customer resides.
- `address` (string, required): The address of the customer.
- `phoneNumber` (PhoneNumber, required): The phone number of the customer.
- `role` (CustomerRole, optional): The role of the customer.
- `products` (IEnumerable<Product>, optional): The products of the customer.
- `orders` (IEnumerable<Order>, optional): The orders of the customer.
- `createAt` (DateTimeOffset, optional): Time of creation.

**Responses:**
- `201 Created`: Customer created successfully.
- `400 Bad Request`: Validation error.
- `401 Unauthorized`
- `403 Forbidden`
- `499 Client Closed Request`
- `500 Internal Server Error`

**Example Request:**
```bash
curl -X POST "https://localhost:44302/api/v1/customers" -H "Content-Type: application/json" \
  -d '{
  "createBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "firstName": "Дмитрий",
  "lastName": "Петров",
  "phoneNumber": {
    "value": "9208785879"
  },
  "cityId": 42,
  "address": "ул. Островского, 20А"
}'
```

**Example Response:**
```bash
{
  "id": "5498c6ae-2e74-40bf-bd13-a765e9d90c9b",
  "firstName": "Дмитрий",
  "middleName": null,
  "lastName": "Петров",
  "fullName": "Петров Дмитрий",
  "phoneNumber": {
    "value": "(920) 878 58 79",
    "countryId": "RU",
    "countryCode": "+7",
    "description": null,
    "fullNumber": "+7 (920) 878 58 79"
  },
  "city": {
    "cityId": 42,
    "name": null,
    "oblast": null,
    "postalCode": null,
    "phoneCode": null
  },
  "address": "ул. Островского, 20А",
  "role": "Владелец",
  "productIds": [],
  "orders": [],
  "products": null,
  "createdAt": "2024-07-07T14:20:32.1554818+03:00",
  "createdBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "lastModifiedAt": null,
  "lastModifiedBy": null,
  "version": 1,
  "isActive": true,
  "isDeleted": false
}
```

#### PATCH `/api/v1/customers/{id}`

Updates a customer by ID.

**Parameters:**
- `id` (string, required): The ID of the customer to update.
- `If-Match` (string, optional): The ETag value representing the version of the customer to be updated.

**Optimistic Concurrency Control:**
- The If-Match header is used to handle optimistic concurrency control. This mechanism helps prevent update conflicts that can occur when multiple clients try to update the same resource simultaneously.
- How It Works: When you retrieve a customer, the response includes an ETag header that represents the current version of the customer. When you attempt to update the customer, you can include this ETag value in the If-Match header. The server compares this value with the current version in the database. If they match, the update proceeds. If they don't match, the server returns a 412 Precondition Failed response, indicating that the resource has been modified by another process since it was last retrieved.

**Request Body:**
- `updateBy` (Guid, required): The ID of the user making the request.
- `firstName` (string, optional): The first name of the customer.
- `middleName` (string, optional): The middle name of the customer.
- `lastName` (string, optional): The last name of the customer.
- `cityId` (int, optional): The ID of the city where the customer resides.
- `address` (string, optional): The address of the customer.
- `phoneNumber` (PhoneNumber, optional): The phone number of the customer.
- `role` (CustomerRole, optional): The role of the customer.
- `products` (IEnumerable<Product>, optional): The products of the customer.
- `orders` (IEnumerable<Order>, optional): The orders of the customer.
- `updateAt` (DateTimeOffset, optional): Time of update.

**Responses:**
- `200 OK`: Customer updated successfully.
- `400 Bad Request`: Validation error.
- `401 Unauthorized`
- `403 Forbidden`
- `404 NotFound`: Customer not found.
- `412 Precondition Failed`: The provided ETag does not match the current version of the customer.
- `499 Client Closed Request`
- `500 Internal Server Error`

**Example Request:**
```bash
curl -X PATCH "https://localhost:44302/api/v1/customers/5498c6ae-2e74-40bf-bd13-a765e9d90c9b" \
  -H "Content-Type: application/json" \
  -H "If-Match: \"1\"" \ 
  -d '{
  "updateBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "phoneNumber": {
    "value": "9150006380"
  },
  "role": "Дилер"
}'
```

**Example Response:**
```bash
{
  "id": "5498c6ae-2e74-40bf-bd13-a765e9d90c9b",
  "firstName": "Дмитрий",
  "middleName": null,
  "lastName": "Петров",
  "fullName": "Петров Дмитрий",
  "phoneNumber": {
    "value": "(915) 000 63 80",
    "countryId": "RU",
    "countryCode": "+7",
    "description": null,
    "fullNumber": "+7 (915) 000 63 80"
  },
  "city": {
    "cityId": 42,
    "name": null,
    "oblast": null,
    "postalCode": null,
    "phoneCode": null
  },
  "address": "ул. Островского, 20А",
  "role": "Дилер",
  "productIds": [],
  "orders": [],
  "products": null,
  "createdAt": "2024-07-07T14:20:32.1554818+03:00",
  "createdBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "lastModifiedAt": "2024-07-07T14:34:59.9061619+03:00",
  "lastModifiedBy": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "version": 3,
  "isActive": true,
  "isDeleted": false
}
```

#### GET `/api/v1/customers/{id}/events`

Fetches customer events by ID.

**Parameters:**
- `id` (string, required): The ID of the customer to fetch events.

**Responses:**
- `200 OK`: Returns the customer events.
- `400 Bad Request`: Validation error.
- `401 Unauthorized`
- `403 Forbidden`
- `404 NotFound`: Customer not found.
- `499 Client Closed Request`
- `500 Internal Server Error`

**Example Request:**
```bash
curl -X GET "https://localhost:44302/api/v1/customers/5498c6ae-2e74-40bf-bd13-a765e9d90c9b/events"
```

**Example Response:**
```bash
{
  "customerCreatedEvent": {
    "firstName": "Дмитрий",
    "middleName": null,
    "lastName": "Петров",
    "fullName": "Петров Дмитрий",
    "phoneNumber": {
      "value": "(920) 878 58 79",
      "countryId": "RU",
      "countryCode": "+7",
      "description": null,
      "fullNumber": "+7 (920) 878 58 79"
    },
    "city": {
      "cityId": 42,
      "name": null,
      "oblast": null,
      "postalCode": null,
      "phoneCode": null
    },
    "address": "ул. Островского, 20А",
    "role": "Владелец",
    "productIds": [],
    "orders": [],
    "createdAt": "2024-07-07T14:20:32.1554818+03:00",
    "customerId": "5498c6ae-2e74-40bf-bd13-a765e9d90c9b",
    "actor": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
  },
  "customerNameChangedEvents": [],
  "customerAddressChangedEvents": [],
  "customerPhoneNumberChangedEvents": [
    {
      "phoneNumber": {
        "value": "(915) 000 63 80",
        "countryId": "RU",
        "countryCode": "+7",
        "description": null,
        "fullNumber": "+7 (915) 000 63 80"
      },
      "phoneNumberChangedAt": "2024-07-07T14:34:59.8994488+03:00",
      "customerId": "5498c6ae-2e74-40bf-bd13-a765e9d90c9b",
      "actor": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    }
  ],
  "customerRoleChangedEvents": [
    {
      "role": "Дилер",
      "roleChangedAt": "2024-07-07T14:34:59.9061619+03:00",
      "customerId": "5498c6ae-2e74-40bf-bd13-a765e9d90c9b",
      "actor": "3fa85f64-5717-4562-b3fc-2c963f66afa6"
    }
  ],
  "customerProductAddedEvents": [],
  "customerProductRemovedEvents": [],
  "customerOrderAddedEvents": [],
  "customerOrderRemovedEvents": [],
  "customerActivatedEvents": [],
  "customerDeactivatedEvents": [],
  "customerDeletedEvents": [],
  "customerUndeletedEvents": []
}
```

#### DELETE `/api/v1/customers/{id}`

Deletes a customer by ID.

**Parameters:**
- `customerId` (string, required): The ID of the customer to fetch.
- `deleteBy` (Guid, required): The ID of the user making the request.

**Responses:**
- `204 No Content`: Customer deleted successfully.
- `400 Bad Request`: Validation error.
- `401 Unauthorized`
- `403 Forbidden`
- `404 NotFound`: Customer not found.
- `499 Client Closed Request`
- `500 Internal Server Error`

**Example Request:**
```bash
curl -X DELETE "https://localhost:44302/api/v1/customers/5498c6ae-2e74-40bf-bd13-a765e9d90c9b?deleteBy=3fa85f64-5717-4562-b3fc-2c963f66afa6"
```

## Notes
- If a field is not included in the update request body, it will remain unchanged.
- All endpoints require appropriate authentication and authorization.
- Ensure proper error handling in your client application to manage different response codes effectively.