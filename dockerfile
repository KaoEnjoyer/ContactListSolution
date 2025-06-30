# Use the official .NET 9 SDK image to build the app
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY *.sln ./
COPY SQLContactsDatabase/*.csproj ./SQLContactsDatabase/
RUN dotnet restore

# Copy everything else and build
COPY . .
WORKDIR /src/SQLContactsDatabase
RUN dotnet publish -c Release -o /app/publish

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "SQLContactsDatabase.dll"]