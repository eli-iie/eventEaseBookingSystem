# EventEase Booking System

A cloud-based event management platform for booking venues and managing events, built with ASP.NET Core MVC and Entity Framework Core. This project was developed as part of the CLDV7111 Cloud Development module (POE Part 3).

## 🚀 Live Demo
[EventEase Booking System on Azure](https://st10129307.azurewebsites.net/Bookings)

## Features
- **Advanced Filtering:**
  - Search bookings by customer, event, or venue
  - Filter by event type, date range, and venue availability
  - Combined and stateful filtering with clear/reset option
- **Venue Management:** CRUD, availability tracking, and capacity management
- **Event Management:** Multiple event types, dynamic assignment, and details
- **Booking System:** Double-booking prevention, customer info, and real-time venue checks
- **Professional UI:** Responsive Bootstrap 5 design, FontAwesome icons, and color-coded status badges

## Getting Started

### 1. Clone the repository
```sh
git clone https://github.com/eli-iie/eventEaseBookingSystem.git
```

### 2. Create your `appsettings.json`
Create a file named `appsettings.json` in the project root with the following structure:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "<YOUR_AZURE_SQL_CONNECTION_STRING>"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

**Note:** Never commit real credentials or secrets to the repository. This file is excluded from git by default.

### 3. Exclude secrets from git
The `.gitignore` file already excludes `appsettings.json` to protect your credentials.

### 4. Run migrations and start the app
```sh
dotnet ef database update
dotnet run
```

### 5. Access the app
Open [http://localhost:5000/Bookings](http://localhost:5000/Bookings) or your Azure deployment URL.

## Technologies Used
- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- Microsoft Azure Web App & Azure SQL Database
- Bootstrap 5, FontAwesome

## Documentation
- All academic and technical documentation is in the `docs/` folder (excluded from git). Only this README is tracked in the repository.

## Author
Eli Beni Ize (ST10129307)

## License
This project is for academic use as part of the IIE CLDV7111 module.
