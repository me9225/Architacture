# Docker Setup for BsdFinalProject

This guide explains how to run the BsdFinalProject (ASP.NET Core 9.0) with SQL Server and Redis using Docker Compose.

## 📋 Prerequisites

- Docker Desktop installed ([Download here](https://www.docker.com/products/docker-desktop))
- Docker Compose (included with Docker Desktop)
- At least 4GB of free disk space

## 🚀 Quick Start

### 1. Build and Start All Services

```powershell
# Navigate to the project root
cd C:\גילי-מסלול\ArcitactureProject2

# Option A: Using docker-compose directly
docker-compose up -d

# Option B: Using the helper script
.\docker-helper.ps1 up
```

### 2. View Logs

```powershell
# View all logs
docker-compose logs -f

# View specific service logs
.\docker-helper.ps1 logs bsd_api
.\docker-helper.ps1 logs sqlserver
.\docker-helper.ps1 logs bsd_redis
```

### 3. Check Status

```powershell
.\docker-helper.ps1 ps
# or
docker-compose ps
```

### 4. Access the Application

- **API**: http://localhost:5000
- **Swagger/OpenAPI**: http://localhost:5000/swagger
- **SQL Server**: `localhost,1433`
- **Redis**: `localhost:6379`

## 🗄️ SQL Server Connection

From SQL Server Management Studio or Azure Data Studio:
- **Server**: `localhost,1433`
- **Username**: `sa`
- **Password**: `YourStrong@Password123`

## 🔴 Redction

### Option 1: Using Redis Helper Script (Recommended)

```powershell
# Interactive menu
.\redis-helper.ps1

# Direct commands
.\redis-helper.ps1 -Command keys
.\redis-helper.ps1 -Command get -Key gift_1
.\redis-helper.ps1 -Command ttl -Key all_gifts
.\redis-helper.ps1 -Command health
```

### Option 2: Manual Connection

```powershell
# Connect to Redis CLI
docker exec -it bsd_redis redis-cli -a YourRedisPassword123

# Common commands (once inside Redis CLI)
KEYS *                    # See all keys
GET gift_1               # Get value
TTL gift_1               # Check TTL
INFO stats               # Statistics
FLUSHALL                 # Clear all keys
```

## 📁 File Structure

```
docker-compose.yml       # Multi-container orchestration
.env                     # Environment variables
.dockerignore           # Files to exclude from Docker build
Dockerfile              # ASP.NET Core application image
docker-helper.ps1       # Docker helper script
redis-helper.ps1        # Redis helper script
DOCKER_SETUP.md         # Detailed Docker guide
REDIS_TESTING_GUIDE.md  # Redis testing guide
REDIS_IMPLEMENTATION.md # Redis implementation details
```

## 🔧 Configuration

### Services Included

| Service | Image | Port | Purpose |
|---------|-------|------|---------|
| SQL Server | mssql/server:2022 | 1433 | Database |
| Redis | redis:7-alpine | 6379 | Cache |
| API | custom (Dockerfile) | 5000 | ASP.NET Core API |

### Update Connection Strings

Edit `.env` or `appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=sqlserver,1433;Database=BsdFinalProjectDb;User Id=sa;Password=YourStrong@Password123;TrustServerCertificate=true;"
}
```

### Change Default Passwords

⚠️ **IMPORTANT FOR PRODUCTION:**

1. Update `.env`:
```
SA_PASSWORD=YourNewStrong@Password456
REDIS_PASSWORD=YourNewRedisPassword789
```

2. Update `docker-compose.yml` and `appsettings.json`

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
.\docker-helper.ps1 clean

# Remove images
docker rmi bsd-api
```

## 💾 Cache (Redis) Features

### How It Works

1. **GET requests** check Redis first (fast)
2. If not found, fetch from database
3. Store result in Redis with TTL (1 hour default)
4. **UPDATE/DELETE** operations invalidate relevant cache

### Test Cache Performance

```powershell
# First call (from database - slow)
curl http://localhost:5000/api/gifts

# Second call (from cache - fast!)
curl http://localhost:5000/api/gifts

# Check what's in Redis
.\redis-helper.ps1 -Command keys

# See TTL
.\redis-helper.ps1 -Command ttl -Key all_gifts
```

### Cache Keys

| Endpoint | Cache Key | TTL |
|----------|-----------|-----|
| GET /api/gifts | all_gifts | 3600s |
| GET /api/gifts/{id} | gift_{id} | 3600s |
| GET /api/gifts/category/{id} | gifts_category_{id} | 3600s |
| GET /api/gifts/cost/{min}/{max} | gifts_cost_{min}_{max} | 3600s |

## 🐛 Troubleshooting

### SQL Server won't start
```powershell
# Check logs
docker-compose logs sqlserver

# Ensure enough disk space and memory
# Restart Docker Desktop
```

### Redis won't connect
```powershell
# Check Redis health
.\redis-helper.ps1 -Command health

# Check logs
docker-compose logs bsd_redis

# Test connection
docker exec bsd_redis redis-cli -a YourRedisPassword123 PING
```

### API can't ct to database
```powershell
# Verify SQL Server is healthy
.\docker-helper.ps1 ps

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

### Build issues
```powershell
# Rebuild with no cache
.\docker-helper.ps1 rebuild

# Or
docker-compose build --no-cache
```

## 📊 Helper Scripts

### Docker Helper
```powershell
.\docker-helper.ps1 up        # Start all containers
.\docker-helper.ps1 down      # Stop all containers
.\docker-helper.ps1 logs      # View logs
.\docker-helper.ps1 ps        # Show status
.\docker-helper.ps1 health    # Check health
.\docker-helper.ps1 rebuild   # Rebuild image
.\docker-helper.ps1 clean     # Stop and remove all
```

### Redis Helper
```powershell
.\redis-helper.ps1                    # Interactive menu
.\redis-helper.ps1 -Command keys      # List all keys
.\redis-helper.ps1 -Command get -Key gift_1
.\redis-helper.ps1 -Command ttl -Key all_gifts
.\redis-helper.ps1 -Command health    # Check Redis health
```

## 📈 Performance Tips

- Allocate sufficient resources to Docker Desktop (4GB+ RAM recommended)
- Use named volumes for persistence
- Monitor memory usage with `docker stats`
- Review logs regularly for errors

## 🔐 Security Notes

- ⚠️ Never commit `.env` with real passwords to Git
- Use strong passwords in production
- Consider using Docker secrets or external secret management
- Restrict Redis access to internal network only

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

# View network info
docker network ls
docker network inspect bsd_network
```

## 🔗 Documentation Files

- **[DOCKER_SETUP.md](./DOCKER_SETUP.md)** - Detailed Docker configuration
- **[REDIS_TESTING_GUIDE.md](./REDIS_TESTING_GUIDE.md)** - Complete Redis testing guide
- **[REDIS_IMPLEMENTATION.md](./REDIS_IMPLEMENTATION.md)** - Redis implementation details

## 🔗 Useful Links

- [Docker Documentation](https://docs.docker.com/)
- [Docker Compose Reference](https://docs.docker.com/compose/compose-file/)
- [SQL Server on Docker](https://hub.docker.com/_/microsoft-mssql-server)
- [Redis on Docker](https://hub.docker.com/_/redis)
- [.NET on Docker](https://hub.docker.com/_/microsoft-dotnet)
- [StackExchange.Redis](https://github.com/StackExchange/StackExchange.Redis)

---

For more information, visit the project repository: https://github.com/me9225/DotnetAngularProject
