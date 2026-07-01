#!/usr/bin/env powershell

# Docker Helper Script
# עזר ב-Docker Compose operations

param(
    [string]$Command = "help",
    [string]$Service = ""
)

function Show-Help {
    Write-Host "`n=== Docker Helper ===" -ForegroundColor Cyan
    Write-Host "`nשימוש: .\docker-helper.ps1 [command] [service]"
    Write-Host "`nפקודות:"
    Write-Host "  up             - הפעל את כל ה-containers"
    Write-Host "  down           - עצור את כל ה-containers"
    Write-Host "  restart        - הפעל מחדש את כל ה-containers"
    Write-Host "  logs [service] - ראה logs (default: all)"
    Write-Host "  ps             - ראה status של containers"
    Write-Host "  clean          - עצור + מחק volumes (⚠️ מוחק DB)"
    Write-Host "  rebuild        - בנה מחדש את ה-image"
    Write-Host "  stats          - ראה resource usage"
    Write-Host "  exec [service] - התחבר ל-container באופן אינטראקטיבי"
    Write-Host "  health         - בדוק בריאות של containers"
    Write-Host "`nדוגמאות:"
    Write-Host "  .\docker-helper.ps1 up"
    Write-Host "  .\docker-helper.ps1 logs bsd_api"
    Write-Host "  .\docker-helper.ps1 exec sqlserver"
}

function Start-AllContainers {
    Write-Host "`n▶️  מפעיל containers..." -ForegroundColor Green
    docker-compose up -d
    Write-Host "`n✅ Containers הפעלו!" -ForegroundColor Green
    docker-compose ps
}

function Stop-AllContainers {
    Write-Host "`n⏹️  עוצר containers..." -ForegroundColor Yellow
    docker-compose down
    Write-Host "`n✅ Containers עצרו!" -ForegroundColor Green
}

function Restart-AllContainers {
    Write-Host "`n🔄 מפעיל מחדש containers..." -ForegroundColor Green
    docker-compose restart
    Write-Host "`n✅ Containers הפעלו מחדש!" -ForegroundColor Green
    docker-compose ps
}

function Show-Logs {
    param([string]$Service = "")

    if ([string]::IsNullOrEmpty($Service)) {
        Write-Host "`n📋 Logs מכל ה-containers:" -ForegroundColor Cyan
        docker-compose logs -f
    }
    else {
        Write-Host "`n📋 Logs מ-$Service:" -ForegroundColor Cyan
        docker-compose logs -f $Service
    }
}

function Show-Status {
    Write-Host "`n📊 Status של containers:" -ForegroundColor Cyan
    docker-compose ps
}

function Clean-Everything {
    $confirm = Read-Host "`n⚠️  זה יעצור ויימחק את הכל כולל DATABASE! בטוח? (yes/no)"

    if ($confirm -eq "yes") {
        Write-Host "`n🧹 מנקה הכל..." -ForegroundColor Red
        docker-compose down -v
        Write-Host "`n✅ הכל נמחק!" -ForegroundColor Green
    }
    else {
        Write-Host "`n❌ ביטול" -ForegroundColor Yellow
    }
}

function Rebuild-Image {
    Write-Host "`n🏗️  בנוי את ה-image מחדש..." -ForegroundColor Green
    docker-compose build --no-cache
    Write-Host "`n✅ Image בנוי מחדש!" -ForegroundColor Green
}

function Show-Stats {
    Write-Host "`n📈 Resource Usage:" -ForegroundColor Cyan
    docker stats --no-stream
}

function Connect-Container {
    param([string]$Service)
    
    if ([string]::IsNullOrEmpty($Service)) {
        $Service = "bsd_api"
    }
    
    Write-Host "`n🔗 התחברות ל-$Service..." -ForegroundColor Green
    
    switch ($Service) {
        "bsd_api" {
            docker exec -it bsd_api powershell
        }
        "sqlserver" {
            docker exec -it bsd_sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Password123"
        }
        "redis" {
            docker exec -it bsd_redis redis-cli -a "YourRedisPassword123"
        }
        default {
            docker exec -it $Service bash
        }
    }
}

function Check-Health {
    Write-Host "`n❤️  בדיקת בריאות:" -ForegroundColor Green

    # Check SQL Server
    Write-Host "`n🗄️  SQL Server:" -ForegroundColor Cyan
    $sqlHealth = docker exec bsd_sqlserver /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "YourStrong@Password123" -Q "SELECT 1" 2>&1
    if ($sqlHealth -like "*1*") {
        Write-Host "✅ SQL Server בריא" -ForegroundColor Green
    }
    else {
        Write-Host "❌ SQL Server בעיות" -ForegroundColor Red
    }

    # Check Redis
    Write-Host "`n🔴 Redis:" -ForegroundColor Cyan
    $redisHealth = docker exec bsd_redis redis-cli -a "YourRedisPassword123" PING 2>&1
    if ($redisHealth -eq "PONG") {
        Write-Host "✅ Redis בריא" -ForegroundColor Green
    }
    else {
        Write-Host "❌ Redis בעיות" -ForegroundColor Red
    }

    # Check API
    Write-Host "`n🌐 API:" -ForegroundColor Cyan
    $apiHealth = curl -s http://localhost:5000/swagger 2>&1
    if ($apiHealth -like "*swagger*") {
        Write-Host "✅ API בריא" -ForegroundColor Green
    }
    else {
        Write-Host "❌ API בעיות" -ForegroundColor Red
    }

    Write-Host "`n📊 Status:" -ForegroundColor Cyan
    docker-compose ps
}

# Main
switch ($Command) {
    "up" { Start-AllContainers }
    "down" { Stop-AllContainers }
    "restart" { Restart-AllContainers }
    "logs" { Show-Logs $Service }
    "ps" { Show-Status }
    "clean" { Clean-Everything }
    "rebuild" { Rebuild-Image }
    "stats" { Show-Stats }
    "exec" { Connect-Container $Service }
    "health" { Check-Health }
    "help" { Show-Help }
    default { 
        Write-Host "❌ פקודה לא ידועה: $Command" -ForegroundColor Red
        Show-Help
    }
}
