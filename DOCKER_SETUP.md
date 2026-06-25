# Docker Setup for BsdFinalProject

This guide explains how to run the BsdFinalProject (ASP.NET Core 9.0) with SQL Server using Docker Compose.

## 📋 Prerequisites

- Docker Desktop installed ([Download here](https://www.docker.com/products/docker-desktop))
- Docker Compose (included with Docker Desktop)
- At least 4GB of free disk space

## 🚀 Quick Start

### 1. Build and Start All Services

```powershell
# Navigate to the project root
cd C:\גילי-מסלול\ArcitactureProject2

# Build and start containers
docker-compose up -d

# Check status
docker-compose ps
```

### 2. View Logs

```powershell
# View all logs
docker-compose logs -f

# View specific service logs
docker-compose logs -f sqlserver
docker-compose logs -f bsd-api
```

### 3. Access the Application

- **API**: http://localhost:5000
- **Swagger/OpenAPI**: http://localhost:5000/swagger
- **SQL Server**: `localhost,1433`

### 4. Connect to SQL Server

From SQL Server Management Studio or Azure Data Studio:
- **Server**: `localhost,1433`
- **Username**: `sa`
- **Password**: `YourStrong@Password123`

## 📁 File Structure

```
docker-compose.yml      # Multi-container orchestration
.env                    # Environment variables
.dockerignore          # Files to exclude from Docker build
Dockerfile             # ASP.NET Core application image
```

## 🔧 Configuration

### Update Connection String

The connection string is automatically configured in `docker-compose.yml`:

```
ConnectionStrings__DefaultConnection: "Server=sqlserver,1433;Database=BsdFinalProjectDb;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=true;"
```

### Modify Ports

Edit `docker-compose.yml` to change port mappings:

```yaml
ports:
  - "5000:80"      # Change first number (external port)
```

### Change Database Password

⚠️ **Important**: For security, change the default password!

1. Update `.env` file:
   ```
   SA_PASSWORD=YourNewStrong@Password456
   ```

2. Update `docker-compose.yml`:
   ```yaml
   SA_PASSWORD: "YourNewStrong@Password456"
   ```

3. Rebuild containers:
   ```powershell
   docker-compose down -v
   docker-compose up -d
   ```

## 🛑 Stop and Clean Up

```powershell
# Stop containers (preserves data)
docker-compose stop

# Stop and remove containers
docker-compose down

# Remove everything including volumes (⚠️ deletes database)
docker-compose down -v

# Remove images
docker rmi bsd-api sqlserver
```

## 🐛 Troubleshooting

### SQL Server won't start
```powershell
# Check logs
docker-compose logs sqlserver

# Ensure enough disk space and memory
# Restart Docker Desktop
```

### API can't connect to database
```powershell
# Verify SQL Server is healthy
docker-compose ps

# Check network connectivity
docker exec bsd_api ping sqlserver
```

### Port already in use
```powershell
# Find process using port 5000
netstat -ano | findstr :5000

# Kill process (replace PID with actual ID)
taskkill /PID <PID> /F

# Or change port in docker-compose.yml
```

## 📊 Performance Tips

- Allocate sufficient resources to Docker Desktop (4GB+ RAM recommended)
- Use named volumes for persistence
- Consider using `.dockerignore` to exclude unnecessary files

## 🔐 Security Notes

- ⚠️ Never commit `.env` with real passwords to Git
- Use strong passwords in production
- Consider using Docker secrets or external secret management

## 📚 Useful Docker Commands

```powershell
# Execute commands in container
docker exec -it bsd_api dotnet --version

# View container stats
docker stats

# Inspect container
docker inspect bsd_api

# Copy files from container
docker cp bsd_api:/app/log.txt ./
```

## 🔗 Useful Links

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Reference](https://docs.docker.com/compose/compose-file/)
- [SQL Server on Docker](https://hub.docker.com/_/microsoft-mssql-server)
- [.NET on Docker](https://hub.docker.com/_/microsoft-dotnet)

---

For more information, visit the project repository: https://github.com/me9225/DotnetAngularProject
