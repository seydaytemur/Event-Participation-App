# EventParticipationApp

This project is an ASP.NET Core MVC web application with event creation and participation tracking features.

## Features

*   **Event Management:** Create, edit, and list events.
*   **Participation Tracking:** Manage user registrations for events.
*   **Database:** Uses SQL Server with Entity Framework Core for ORM support.

## Technologies

*   .NET 8.0
*   ASP.NET Core MVC
*   Entity Framework Core (SQL Server)

## Setup and Execution

1.  Clone the project to your local environment.
2.  Update the `ConnectionStrings:DefaultConnection` section in `appsettings.json` to match your SQL Server configuration.
3.  To create the database, run the following command in the project directory:
    ```bash
    dotnet ef database update
    ```
4.  Run the application:
    ```bash
    dotnet run
    ```

## Project Structure

*   `Controllers`: Handles application logic and API endpoints.
*   `Models`: Defines data structures and database tables.
*   `Views`: Contains the user interface pages.
*   `Data`: Manages database context (DbContext).
