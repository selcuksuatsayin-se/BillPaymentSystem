# Mobile Provider Bill Payment API

This project is the Backend API application of the **Mobile Operator Bill Payment System** developed project.

The project includes RESTful services that manage subscribers invoice querying, banks integration, and admins invoice uploading processes.

## 🚀 Live Demo (Live URL)
The project is live on Azure App Service :   
**Swagger UI:** [https://billpayment-api-selcuk-fah3erbdeuhva6e8.canadacentral-01.azurewebsites.net/swagger](https://billpayment-api-selcuk-fah3erbdeuhva6e8.canadacentral-01.azurewebsites.net/swagger)

---

## 🏗 Architectural and Design Decisions

The project is built on a **Monolithic** structure and operates an **API Gateway** logic using **Modular** principles and **Middleware** architecture.

### Core Components
***API Gateway (Middleware):** Instead of a separate server, *Rate Limiting* and *Logging* operations are managed centrally with special middleware running at the project's entry level.
***Authentication:** JWT (JSON Web Token) is used. [cite_start]Admin and User roles are separated.
***Database:** Azure SQL Database (using a Code-First approach).
***Logging:** All Request/Response traffic (Headers, Body sizes, Latency, etc.) is stored in the `ApiLogs` table in the database.

### Kullanılan Teknolojiler
* .NET 8 Web API
* Entity Framework Core (SQL Server)
* JWT Bearer Authentication
* Swagger / OpenAPI (Swashbuckle Annotations)
* CsvHelper (For batch processing)

---

## ✅ Tamamlanan Gereksinimler

| Feature | Status | Description |
| :--- | :--- | :--- |
| **Admin Batch Upload** | ✅ |  Bulk invoice upload from CSV file |
| **Single Bill Add** | ✅ | Single invoice creation |
| **Query Bill** | ✅ | Subscriber query with token (Limited: 3 per day) |
| **Query Bill Detailed** | ✅ |  Detailed paging query (Unlimited)) |
| **Banking Integration** | ✅ |  External debt query with phone number |
| **Payment** | ✅ |  Token-free, partial payment support |
| **Rate Limiting** | ✅ |  MemoryCache-based limiting for `Query Bill` endpoint |
| **Comprehensive Logging**| ✅ |  Request/Response times, IP, Path, Status Code recording |

---

## 🛠 Installation (Local)

To run the project on your own computer:

1. Clone the repo.
2. Update the `ConnectionStrings` field in the `appsettings.json` file to your SQL Server information.
3. Run the following command in the terminal (creates the database):
```bash
dotnet ef database update
```
4. Start the project:
```bash
dotnet run
```

---

## 🧪 Test Scenarios

**1. Admin Login & Upload Invoices:**
* Obtain tokens via `/api/v1/Auth/login` using `**********`.
* Enter the token in the Authorize button.
* Upload invoices using the `Admin` endpoints.

**2. Rate Limiting Test:**
* Log in as any subscriber.
* Send four consecutive requests to the `/api/v1/Subscriber/bills` endpoint.
* The 4th request will result in a **429 Too Many Requests** error.

**3. Payment (No Auth):**
* You can make payments without using tokens via the `/api/v1/Payment/pay` endpoint.
