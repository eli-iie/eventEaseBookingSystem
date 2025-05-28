# eventEaseBookingSystem

## Live Website

[EventEase Booking System - Live Demo](https://eventeasebookingsystem20250528105136-gtf0anbkbngsbhfz.southafricanorth-01.azurewebsites.net/)

---

## Project Overview

This is an ASP.NET Core MVC application for managing event bookings, venues, and reports. It uses Azure SQL Database and Azure Blob Storage for data and file storage.

---

## How to Run Locally

1. **Clone the repository:**
   ```sh
   git clone https://github.com/eli-iie/eventEaseBookingSystem.git
   cd eventEaseBookingSystem
   ```

2. **Restore dependencies:**
   ```sh
   dotnet restore
   ```

3. **Configure Database:**

   Option A - Using Local SQL Server Express (Recommended for Marking):
   - Install SQL Server Express if you haven't already
   - Open appsettings.json and replace the connection string with:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=EventEaseDB;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```
   - The application will create the database and seed initial data on first run

   Option B - Using Azure Resources (Production):
   - Contact the project owner for Azure connection strings
   - Use dotnet user-secrets to store them:
   ```sh
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<request-from-project-owner>"
   dotnet user-secrets set "AzureBlobStorage:ConnectionString" "<request-from-project-owner>"
   ```

   Note: File uploads will be stored locally when using SQL Express, and in Azure Blob Storage when using Azure connection strings.

4. **Run the application:**
   ```sh
   dotnet run --project eventEaseBookingSystem/eventEaseBookingSystem.csproj
   ```
   The app will be available at `http://localhost:5220` by default.

---

## Publishing & Secrets

- **Never commit real secrets to the repository.**
- Use `dotnet user-secrets` for local development.
- For production, set secrets in your Azure App Service configuration.

---

## Notes for Lecturer

- All secrets are excluded from the repository. Use the commands above to add them locally if you wish to run the project.
- The live demo is available at the link above.
- If you need further help, please contact the student.

---

## License

This project is for educational purposes only.
