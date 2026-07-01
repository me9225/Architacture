#!/usr/bin/env powershell

# Redis Helper Script
# טלעזרה בהתחברות ל-Redis וביצוע פקודות

param(
    [string]$Command = "cli",
    [string]$Key = "",
    [string]$Pattern = "*",
    [string]$Password = "YourRedisPassword123"
)

$RedisContainer = "bsd_redis"
$RedisPort = 6379

function Show-Menu {
    Write-Host "`n=== Redis Helper Menu ===" -ForegroundColor Cyan
    Write-Host "1. התחבר ל-Redis CLI"
    Write-Host "2. ראה את כל ה-Keys"
    Write-Host "3. ראה מידע על Key ספציפי"
    Write-Host "4. ראה TTL של Key"
    Write-Host "5. מחק Key"
    Write-Host "6. ראה סטטיסטיקה"
    Write-Host "7. ראה Memory Usage"
    Write-Host "8. בדוק Redis Health"
    Write-Host "9. צא"
}

function Connect-RedisCLI {
    Write-Host "`nמתחבר ל-Redis CLI..." -ForegroundColor Green
    docker exec -it $RedisContainer redis-cli -a $Password
}

function Get-AllKeys {
    Write-Host "`nכל ה-Keys ב-Redis:" -ForegroundColor Green
    docker exec $RedisContainer redis-cli -a $Password KEYS "*"
}

function Get-KeyValue {
    param([string]$KeyName)

    if ([string]::IsNullOrEmpty($KeyName)) {
        $KeyName = Read-Host "הזן את שם ה-Key"
    }

    Write-Host "`nערך ה-Key: $KeyName" -ForegroundColor Green
    $value = docker exec $RedisContainer redis-cli -a $Password GET $KeyName

    if ([string]::IsNullOrEmpty($value) -or $value -eq "(nil)") {
        Write-Host "❌ Key לא קיים" -ForegroundColor Red
    }
    else {
        Write-Host "✅ Key קיים:" -ForegroundColor Green
        Write-Host $value | ConvertFrom-Json | ConvertTo-Json -Depth 10
    }
}

function Get-KeyTTL {
    param([string]$KeyName)

    if ([string]::IsNullOrEmpty($KeyName)) {
        $KeyName = Read-Host "הזן את שם ה-Key"
    }

    Write-Host "`nTTL של ה-Key: $KeyName" -ForegroundColor Green
    $ttl = docker exec $RedisContainer redis-cli -a $Password TTL $KeyName

    if ($ttl -eq -1) {
        Write-Host "⏰ Key קיים אבל אין TTL (יעמוד לעולמים)" -ForegroundColor Yellow
    }
    elseif ($ttl -eq -2) {
        Write-Host "❌ Key לא קיים" -ForegroundColor Red
    }
    else {
        Write-Host "⏰ TTL: $ttl שניות ($([Math]::Round($ttl / 60, 2)) דקות)" -ForegroundColor Green
    }
}

function Remove-RedisKey {
    param([string]$KeyName)

    if ([string]::IsNullOrEmpty($KeyName)) {
        $KeyName = Read-Host "הזן את שם ה-Key למחיקה"
    }

    $confirm = Read-Host "בטוח רוצה למחוק את $KeyName? (y/n)"

    if ($confirm -eq "y") {
        docker exec $RedisContainer redis-cli -a $Password DEL $KeyName | Out-Null
        Write-Host "✅ Key נמחק בהצלחה" -ForegroundColor Green
    }
}

function Get-RedisStats {
    Write-Host "`nRedis Stats:" -ForegroundColor Green
    docker exec $RedisContainer redis-cli -a $Password INFO stats
}

function Get-MemoryUsage {
    Write-Host "`nMemory Usage:" -ForegroundColor Green
    docker exec $RedisContainer redis-cli -a $Password INFO memory
}

function Test-RedisHealth {
    Write-Host "`nבדיקת בריאות Redis..." -ForegroundColor Green

    $ping = docker exec $RedisContainer redis-cli -a $Password PING

    if ($ping -eq "PONG") {
        Write-Host "✅ Redis בריא וענה PONG" -ForegroundColor Green
    }
    else {
        Write-Host "❌ בעיה בתגובה מ-Redis" -ForegroundColor Red
    }

    # בדוק כמה Keys
    $keyCount = docker exec $RedisContainer redis-cli -a $Password DBSIZE
    Write-Host "📊 סה\"כ Keys: $keyCount" -ForegroundColor Cyan

    # בדוק Memory
    $memory = docker exec $RedisContainer redis-cli -a $Password INFO memory | Select-String "used_memory_human"
    Write-Host $memory -ForegroundColor Cyan
}

# Main Menu Loop
if ($Command -eq "cli") {
    while ($true) {
        Show-Menu
        $choice = Read-Host "`nבחר אפשרות"

        switch ($choice) {
            "1" { Connect-RedisCLI }
            "2" { Get-AllKeys }
            "3" { Get-KeyValue $Key }
            "4" { Get-KeyTTL $Key }
            "5" { Remove-RedisKey $Key }
            "6" { Get-RedisStats }
            "7" { Get-MemoryUsage }
            "8" { Test-RedisHealth }
            "9" { 
                Write-Host "יציאה..." -ForegroundColor Yellow
                exit 
            }
            default { Write-Host "❌ אפשרות לא ידועה" -ForegroundColor Red }
        }
    }
}
elseif ($Command -eq "keys") {
    Get-AllKeys
}
elseif ($Command -eq "get") {
    Get-KeyValue $Key
}
elseif ($Command -eq "ttl") {
    Get-KeyTTL $Key
}
elseif ($Command -eq "del") {
    Remove-RedisKey $Key
}
elseif ($Command -eq "stats") {
    Get-RedisStats
}
elseif ($Command -eq "memory") {
    Get-MemoryUsage
}
elseif ($Command -eq "health") {
    Test-RedisHealth
}
else {
    Write-Host "❌ פקודה לא ידועה: $Command" -ForegroundColor Red
    Write-Host "`nשימוש: .\redis-helper.ps1 [command] [options]" -ForegroundColor Yellow
    Write-Host "`nפקודות:"
    Write-Host "  cli                    - פתח Redis CLI אינטראקטיבי"
    Write-Host "  keys                   - ראה את כל ה-Keys"
    Write-Host "  get -Key <name>        - ראה ערך של Key"
    Write-Host "  ttl -Key <name>        - ראה TTL של Key"
    Write-Host "  del -Key <name>        - מחק Key"
    Write-Host "  stats                  - ראה סטטיסטיקה"
    Write-Host "  memory                 - ראה Memory Usage"
    Write-Host "  health                 - בדוק בריאות"
}
