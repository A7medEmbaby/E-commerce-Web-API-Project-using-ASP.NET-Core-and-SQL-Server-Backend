# 🌐 E-Commerce Real-Time Application using ASP.NET Core Web API

Welcome to the repository of an **eCommerce real-time application** built with **ASP.NET Core Web API**. This application is designed to manage various entities such as customers, products, orders, and payments, focusing on scalable architecture and efficient performance.

## 🚀 Features

- **Customer Management**: Add, update, and soft-delete customers with the `IsDeleted` flag.
- **Product Catalog**: Comprehensive CRUD operations for product listings.
- **Order Processing**: Create and manage customer orders and their items seamlessly.
- **Payments Integration**: Efficient handling of payments linked to orders.
- **Real-Time Updates**: Supports real-time data updates for enhanced user experience.
- **Entity Framework Core**: Optimized data access layer for high performance.
- **AutoMapper**: Simplifies mapping between database entities and DTOs.

## 🛠️ Technology Stack

- **Backend**: ASP.NET Core Web API
- **Database**: SQL Server using Entity Framework Core
- **Tools**: 
  - **AutoMapper**: For object mapping
  - **Dependency Injection**: For better service management
  - **Fluent Validation**: For request validation
  - **Swagger**: For API documentation and testing

## 📋 Prerequisites

- .NET 8 SDK
- SQL Server
- Visual Studio or any preferred code editor

## 📥 Setup Instructions

1. **Clone the repository:**
    ```bash
    git clone https://github.com/A7medEmbaby/E-commerce-Web-API-Project-using-ASP.NET-Core-and-SQL-Server-Backend.git
    cd E-commerce-Web-API-Project-using-ASP.NET-Core-and-SQL-Server-Backend
    ```

2. **Update the connection string in `appsettings.json`:**
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=your_server;Database=ecommerce_db;Trusted_Connection=True;"
    }
    ```

3. **Restore dependencies:**
    ```bash
    dotnet restore
    ```

4. **Run database migrations:**
    ```bash
    dotnet ef database update
    ```

5. **Run the application:**
    ```bash
    dotnet run
    ```

6. **Access Swagger documentation**: Navigate to `https://localhost:5001/swagger` to view available endpoints and interact with the API.

## 📡 Endpoints

### Customer
- **GET** `/api/Customer/All` - Retrieve all customers.
- **GET** `/api/Customer/ById/{id}` - Retrieve a customer by ID.
- **POST** `/api/Customer/Create` - Create a new customer.
- **PUT** `/api/Customer/{id}/Update` - Update an existing customer.
- **DELETE** `/api/Customer/{id}/Delete` - Soft delete a customer.

### Order
- **GET** `/api/Order/All` - Retrieve all orders.
- **POST** `/api/Order/Create` - Create a new order.
- **GET** `/api/Order/ById/{id}` - Retrieve an order by ID.
- **PUT** `/api/Order/{id}/status` - Update the status of an existing order.
- **PUT** `/api/Order/{id}/confirm` - Confirm an existing order.

### Payment
- **POST** `/api/Payment/MakePayment` - Make a payment for an order.
- **GET** `/api/Payment/PaymentDetails/{id}` - Retrieve payment details by ID.
- **PUT** `/api/Payment/UpdatePaymentStatus/{id}` - Update the status of a payment.

### Product
- **GET** `/api/Product/All` - Retrieve all products.
- **GET** `/api/Product/ById/{id}` - Retrieve a product by ID.
- **POST** `/api/Product/Create` - Create a new product.
- **PUT** `/api/Product/{id}/Update` - Update an existing product.
- **DELETE** `/api/Product/{id}/Delete` - Delete a product.

## 🏗️ Architecture

The project follows the **clean architecture** design pattern:

- **Controllers**: Manage HTTP requests and responses.
- **Services**: Encapsulate business logic and interact with repositories.
- **Repositories**: Handle data access using Entity Framework Core.
- **DTOs (Data Transfer Objects)**: Decouple internal models from API responses.

## 📞 Contact Information

For questions, feedback, or support, feel free to reach out:

- **Name**: Ahmed Embaby
- [a7medembaby@gmail.com](mailto:a7medembaby@gmail.com)
- [GitHub](https://github.com/A7medEmbaby)
- [LinkedIn](https://www.linkedin.com/in/ahmed-m-embaby)

I welcome any inquiries or collaboration opportunities!

## 🤝 Contributing

Contributions are welcome! Please open an issue or submit a pull request for any features or improvements.
