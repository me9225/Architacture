# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY ["BsdFinalProject/BsdFinalProject.csproj", "BsdFinalProject/"]
RUN dotnet restore "BsdFinalProject/BsdFinalProject.csproj"

# Copy source code
COPY . .
WORKDIR "/src/BsdFinalProject"

# Build the application
RUN dotnet build "BsdFinalProject.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "BsdFinalProject.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Expose port (change if your app uses a different port)
EXPOSE 80

# Set environment variable for ASP.NET Core
ENV ASPNETCORE_ENVIRONMENT=Production
ENV ASPNETCORE_URLS=http://+:80

# Start the application
ENTRYPOINT ["dotnet", "BsdFinalProject.dll"]
