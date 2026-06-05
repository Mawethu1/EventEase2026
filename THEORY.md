Component Discussion: Azure Services Used and Why

To complete this phase of the project, I used several Azure services that worked together to provide a fully cloud-hosted solution.

Azure SQL Database

I chose Azure SQL Database because it provides a managed relational database service that integrates well with Entity Framework Core. It allowed me to store all venue, event, booking, and event type data in a secure and scalable environment. I also used it to host the EventTypes lookup table and the vw_BookingDetails view that supports the application’s search functionality.

Azure Storage Account

I used Azure Storage Account to store venue images in Blob Storage. During development I relied on Azurite, but moving to Azure Storage allowed me to work with a real cloud storage solution. This ensured that uploaded images could be stored and accessed through a reliable cloud endpoint.

Azure App Service

I deployed my ASP.NET Core MVC application to Azure App Service. I selected this service because it provides a managed hosting environment and removes the need for me to configure and maintain my own web server. It also provides HTTPS support, application management features, and integration with Azure configuration settings.

I selected these services because together they addressed all the requirements of my application. Azure SQL Database manages the relational data, Azure Storage manages media files, and Azure App Service hosts the web application.

The Migration Experience

Moving my application from a local environment to Azure was one of the most valuable learning experiences during this project. While the application functioned correctly on my local machine, deploying it to the cloud required additional planning, configuration, and testing.

Database Migration

During development I used SQL Server LocalDB. To prepare the application for production, I migrated the database structure and data to Azure SQL Database. This involved creating the cloud database, updating connection strings, applying Entity Framework migrations, and verifying that all tables, relationships, lookup data, and views were created correctly.

Through this process, I gained a better understanding of how cloud-hosted databases differ from local development databases and how migrations can be used to keep database structures consistent across environments.

Storage Migration

For image storage, I initially used Azurite as a local emulator. When moving to production, I created a real Azure Storage Account and configured a blob container for venue images. I then replaced the development connection strings with production values and tested image uploads and downloads to ensure everything worked correctly.

This taught me the importance of properly managing cloud storage resources and validating that application features continue to function after deployment.

App Service Deployment

Deploying the application to Azure App Service was the final stage of the migration process. I published the application in Release mode, packaged the deployment files, and deployed them to Azure.

After deployment, I verified that the application was using the correct production database and storage resources. This helped me understand the complete deployment lifecycle of a cloud-hosted web application.

Configuration Changes

One of the most important lessons I learned was the importance of environment-specific configuration. I maintained separate settings for development and production environments to ensure that local resources were not accidentally used in production.

By doing this, I was able to switch between environments without changing the application code, which made deployment safer and easier to manage.