# ---------------------------------------------
# 1. בסיס .NET 8 SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /app

# העתקת קבצי הפרויקט
COPY . .

# בניית הפרויקט
RUN dotnet restore
RUN dotnet build -c Release --no-restore

# ---------------------------------------------
# 2. שלב ריצה
FROM mcr.microsoft.com/dotnet/runtime:8.0

WORKDIR /app

# התקנת Chrome + ספריות דרושות
RUN apt-get update && \
    apt-get install -y wget unzip curl xvfb gnupg2 ca-certificates lsb-release && \
    wget -q -O - https://dl.google.com/linux/linux_signing_key.pub | gpg --dearmor > /usr/share/keyrings/google-linux-signing-key.gpg && \
    echo "deb [arch=amd64 signed-by=/usr/share/keyrings/google-linux-signing-key.gpg] http://dl.google.com/linux/chrome/deb/ stable main" > /etc/apt/sources.list.d/google.list && \
    apt-get update && \
    apt-get install -y google-chrome-stable

# התקנת ChromeDriver התואם לגרסת Chrome
RUN CHROME_VERSION=$(google-chrome --version | awk '{print $3}' | cut -d'.' -f1) && \
    LATEST_DRIVER=$(curl -sS https://chromedriver.storage.googleapis.com/LATEST_RELEASE_$CHROME_VERSION) && \
    wget -O /tmp/chromedriver.zip "https://chromedriver.storage.googleapis.com/$LATEST_DRIVER/chromedriver_linux64.zip" && \
    unzip /tmp/chromedriver.zip -d /usr/local/bin/ && \
    chmod +x /usr/local/bin/chromedriver

# העתקת הקבצים שבנינו לשלב הריצה
COPY --from=build /app/bin/Release/net8.0 /app

# הפעלת הקובץ הראשי
CMD ["dotnet", "Data_From_Html.dll"]
