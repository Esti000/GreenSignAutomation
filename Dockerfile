# Use the official .NET 8 SDK image to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set working directory inside the container
WORKDIR /app

# Copy everything to the container
COPY . ./

# Restore NuGet packages
RUN dotnet restore

# Build the project in Release mode
RUN dotnet build -c Release --no-restore

# Publish the project to a folder
RUN dotnet publish -c Release --no-build -o out

# Use the official .NET 8 runtime image for running
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime

WORKDIR /app

# Copy the published files from the build stage
COPY --from=build /app/out ./

# Set the entry point
ENTRYPOINT ["dotnet", "Data_From_Html.dll"]
