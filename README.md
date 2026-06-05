Part 3 extends the application from a local development project to a cloud-hosted solution running on Azure.

Features Added

* Event type management using a dedicated EventTypes lookup table.
* Advanced search and filtering by:
    * Event Type
    * Venue
    * Date Range
    * Available Bookings
* Venue image upload and storage using Azure Blob Storage.
* Admin authentication using cookie-based login.
* Booking search through the vw_BookingDetails SQL view.
* Deployment to Azure App Service.
* Migration from LocalDB to Azure SQL Database.

Authentication

* AccountController handles administrator login and logout.
* Admin credentials are configured through appsettings.json and appsettings.Production.json.
* Protected pages require authentication and redirect unauthenticated users to /Account/Login.

Azure Services

The application uses the following Azure services:

* Azure SQL Database for relational data storage.
* Azure Storage Account (Blob Storage) for venue images.
* Azure App Service for hosting the ASP.NET Core MVC application.

Deployment

To deploy the application:

1. Build and publish the project in Release mode.
2. Generate the publish output.
3. Deploy the published files or ZIP package to Azure App Service.
4. Configure the Azure SQL Database and Azure Storage connection strings.
5. Verify that the application can access the database and blob storage resources.


