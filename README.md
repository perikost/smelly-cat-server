
# Retail Store Management System (Backend)
## Overview
This backend component of the Retail Store Management System is responsible for handling the server-side logic and API endpoints. The system allows the store owner to manage products, customers, and purchases.

## Technologies Used
- .NET 7: The backend is developed using .NET 7.
- Entity Framework Core: Database interactions are managed through Entity Framework.
- SQL Server (MSSQL): The backend utilizes MSSQL as the relational database to store product, customer, and purchase data.

## Getting Started
Follow these steps to set up and run the server:

Clone the Repository:

```
git clone https://github.com/perikost/smelly-cat-server.git
cd smelly-cat-server
```

Configure Database Connection:

- Open the appsettings.json file.
- Modify the connection string under "DataContext.cs" to point to your MSSQL database.

Install the command-line interface (CLI) tools for Entity Framework Core:
```
dotnet tool install --global dotnet-ef

```

Run Migrations:

```
dotnet ef database update
```

Run the Application:

```
dotnet run
```

The backend will start running at http://localhost:8080.

## API Endpoints
The available endpoints can be found [here](http://localhost:8080/swagger/index.html), after running the server.
