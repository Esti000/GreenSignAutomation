# 1. בסיס .NET 8 SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# 2. תיקיית עבודה
WORKDIR /app

# 3. העתקת קבצי הפרויקט
COPY . .

# 4. בניית הפרויקט
RUN dotnet restore
RUN dotnet build -c Release --no-restore

# ---------------------------------------------
# שלב ריצה
FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app

# 5. התקנת Chrome + ספריות דרושות
RUN apt-get update && \
    apt-get install -y wget unzip curl xvfb gnupg2 ca-certificates && \
    wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | apt-key add - && \
    echo "deb [arch=amd64] http://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google.list && \
    apt-get update && \
    apt-get install -y google-chrome-stable

# 6. התקנת ChromeDriver
RUN LATEST=$(curl -sS chromedriver.storage.googleapis.com/LATEST_RELEASE) && \
    wget -O /tmp/chromedriver.zip "https://chromedriver.storage.googleapis.com/$LATEST/chromedriver_linux64.zip" && \
    unzip /tmp/chromedriver.zip -d /usr/local/bin/ && \
    chmod +x /usr/local/bin/chromedriver

# 7. העתקת הקבצים שבנינו לשלב הריצה
COPY --from=build /app/bin/Release/net8.0 /app

# 8. הפעלת הקובץ הראשי
CMD ["dotnet", "Data_From_Html.dll"]
