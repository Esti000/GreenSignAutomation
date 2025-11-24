# ===========================
# שלב 1: Build
# ===========================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# העתק את קובץ ה-sln וכל קבצי csproj
COPY *.sln ./
COPY **/*.csproj ./

# התקן את התלויות
RUN dotnet restore

# העתק את כל הקוד
COPY . ./

# Build ו-Publish לתיקייה 'out'
RUN dotnet publish -c Release -o out

# ===========================
# שלב 2: Runtime
# ===========================
FROM mcr.microsoft.com/dotnet/runtime:8.0 AS runtime
WORKDIR /app

# העתק את הפלט מה-build
COPY --from=build /app/out ./

# הפעלת האפליקציה
ENTRYPOINT ["dotnet", "GreenSignAutomation.dll"]
