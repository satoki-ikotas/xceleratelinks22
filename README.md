# xceleratelinks22

## Solution structure
- `APIPSI16` – ASP.NET Core API project
- `XcelerateLinks` – MVC front-end project
- `XcelerateLinks_DTOs` – shared DTOs

## Getting started
1. Restore and build:
   ```bash
   dotnet restore
   dotnet build APIPSI16.sln
   ```
2. Configure environment variables:
   - `ConnectionStrings__DefaultConnection`
   - `XCELERATE_JWT_KEY`
   - `Api__BaseUrl`
3. Run the API and MVC apps in separate terminals:
   ```bash
   dotnet run --project APIPSI16
   dotnet run --project XcelerateLinks
   ```
