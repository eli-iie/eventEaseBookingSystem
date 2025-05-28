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

3. **Configure Secrets:**
   - This project uses [User Secrets](https://learn.microsoft.com/en-us/aspnet/core/security/app-secrets) for sensitive information.
   - The following secrets are required (replace with your own or request from the project owner):
     - `ConnectionStrings:DefaultConnection` = `***REMOVED***;`
     - `AzureBlobStorage:ConnectionString` = `DefaultEndpointsProtocol=https;AccountName=eventeasestore10129307;AccountKey=***REMOVED***;EndpointSuffix=core.windows.net`
   - Do NOT commit `appsettings.json` with real secrets to the repository.
   - Instead, add secrets locally using the following commands:

   ```sh
   dotnet user-secrets init
   dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<YOUR-DB-CONNECTION-STRING-HERE>"
   dotnet user-secrets set "AzureBlobStorage:ConnectionString" "<YOUR-AZURE-BLOB-CONNECTION-STRING-HERE>"
   ```

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
