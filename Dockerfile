# ---------------------------------------------
# 1. בסיס .NET 8 SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# 2. העתקת קבצי הפרויקט
COPY . .

# 3. בניית הפרויקט
RUN dotnet restore
RUN dotnet build -c Release --no-restore

# ---------------------------------------------
# שלב ריצה
FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app

# 4. התקנת Chrome + ספריות דרושות
RUN apt-get update && \
    apt-get install -y wget unzip curl gnupg2 ca-certificates fonts-liberation libnss3 libxss1 libx11-xcb1 libxcomposite1 libxdamage1 libxrandr2 libxext6 libglib2.0-0 xvfb && \
    wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | gpg --dearmor > /usr/share/keyrings/google-linux-signing-key.gpg && \
    echo "deb [arch=amd64 signed-by=/usr/share/keyrings/google-linux-signing-key.gpg] http://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google.list && \
    apt-get update && \
    apt-get install -y google-chrome-stable

# 5. התקנת ChromeDriver
RUN LATEST=$(curl -sS chromedriver.storage.googleapis.com/LATEST_RELEASE) && \
    wget -O /tmp/chromedriver.zip "https://chromedriver.storage.googleapis.com/$LATEST/chromedriver_linux64.zip" && \
    unzip /tmp/chromedriver.zip -d /usr/local/bin/ && \
    chmod +x /usr/local/bin/chromedriver

# 6. העתקת הקבצים שבנינו לשלב הריצה
COPY --from=build /app/bin/Release/net8.0 /app

# 7. הפעלת הקובץ הראשי
CMD ["dotnet", "Data_From_Html.dll"]
