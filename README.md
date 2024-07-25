.vs/
Bookstore API
A Modern Bookstore Management System
Introduction
Welcome to the Bookstore API project! This API serves as the backend for a modern bookstore application, providing functionalities for managing books, authors, categories, customers, and order history. The API supports full CRUD operations for admins and read-only access for customers, with robust JWT-based authentication and authorization.

 <!-- Add the path to your visual helper image here -->

Features
JWT Authentication: Secure API access with JWT-based authentication.
Role-Based Authorization: Different access levels for admins and customers.
CRUD Operations: Full CRUD operations for books, authors, and categories.
Search and Filter: Advanced search and filtering options.
Order Management: Handle book purchases and order history.
Caching: Improved performance with memory caching.
User Instructions
Prerequisites
.NET 6.0 SDK or later
SQL Server
Getting Started
Clone the repository:

bash
Copy code
git clone https://github.com/yourusername/bookstore-api.git
cd bookstore-api
Set up the database:

Update the connection string in appsettings.json to match your SQL Server setup.

json
Copy code
"ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=Bookstore;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
}
Run the migrations:

bash
Copy code
dotnet ef database update
Build and run the application:

bash
Copy code
dotnet build
dotnet run
Access the API:

The API will be accessible at https://localhost:5001 (or the port configured in your launch settings).

Developer Instructions
Setting Up the Development Environment
Install dependencies:

bash
Copy code
dotnet restore
Running the application:

Use the following command to run the application in development mode:

bash
Copy code
dotnet watch run
Testing:

Run the unit tests using:

bash
Copy code
dotnet test
Swagger Documentation:

Access the Swagger UI for API documentation at https://localhost:5001/swagger when running in development mode.

Contributor Expectations
We welcome contributions from the community! Please ensure you follow these guidelines:

Code Quality: Maintain high code quality and follow the existing coding standards.
Testing: Write unit tests for new features and bug fixes.
Documentation: Update the documentation for any changes or new features.
Pull Requests: Provide clear descriptions and follow the pull request template.
Known Issues
Issue 1: JWT tokens are not invalidated upon user logout. Workaround: Manually manage token invalidation.
Issue 2: Search performance may degrade with a large dataset. Workaround: Optimize indexing and consider implementing full-text search.
Future Enhancements
Feature 1: Add support for user roles beyond admin and customer.
Feature 2: Implement token blacklisting for enhanced security.
Feature 3: Add a recommendation engine based on user preferences and purchase history.
