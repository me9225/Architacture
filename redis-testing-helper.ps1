# Redis Testing Helper Script
# משתמש: redis-cli כדי להתחבר ל-Redis container וביצוע פעולות

param(
    [Parameter(Mandatory = $false)]
    [string]$Command = ""
)

# Configuration
$CONTAINER_NAME = "bsd_redis"
$PASSWORD = "YourRedisPassword123"

function Connect-Redis {
    param(
        [Parameter(Mandatory = $false)]
        [string]$RedisCommand = ""
    )
    
    if ([string]::IsNullOrWhiteSpace($RedisCommand)) {
        # Interactive mode
        Write-Host "🔓 כניסה ל-Redis CLI (Interactive Mode)" -ForegroundColor Green
        Write-Host "הקלד את הפקודות שלך. קלד EXIT או QUIT כדי לצאת." -ForegroundColor Cyan
        Write-Host ""
        docker exec -it $CONTAINER_NAME redis-cli -a $PASSWORD
    }
    else {
        # Execute specific command
        docker exec $CONTAINER_NAME redis-cli -a $PASSWORD $RedisCommand
    }
}

function Show-AllKeys {
    Write-Host "📋 הצגת כל ה-Keys ב-Redis:" -ForegroundColor Green
    Connect-Redis "KEYS *"
}

function Show-KeyValue {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Key
    )
    
    Write-Host "📖 ערך של Key: $Key" -ForegroundColor Green
    Connect-Redis "GET $Key"
}

function Show-KeyTTL {
    param(
        [Parameter(Mandatory = $true)]
        [string]$Key
    )
    
    Write-Host "⏱️  TTL של Key: $Key" -ForegroundColor Green
    Connect-Redis "TTL $Key"
}

function Show-RedisInfo {
    Write-Host "ℹ️  מידע כללי על Redis:" -ForegroundColor Green
    Connect-Redis "INFO"
}

function Show-KeyCount {
    Write-Host "🔢 מספר Keys בקיימות:" -ForegroundColor Green
    Connect-Redis "DBSIZE"
}

function Clear-AllKeys {
    Write-Host "⚠️  הסרת כל ה-Keys..." -ForegroundColor Yellow
    $confirm = Read-Host "האם אתה בטוח? (yes/no)"
    if ($confirm -eq "yes") {
        Connect-Redis "FLUSHALL"
        Write-Host "✅ כל ה-Keys נמחקו" -ForegroundColor Green
    }
    else {
        Write-Host "❌ ביטול" -ForegroundColor Red
    }
}

function Show-Menu {
    Write-Host ""
    Write-Host "========== Redis Testing Helper Menu ==========" -ForegroundColor Cyan
    Write-Host "1. KEYS * - הצגת כל ה-Keys" -ForegroundColor White
    Write-Host "2. GET <key> - הצגת ערך של Key" -ForegroundColor White
    Write-Host "3. TTL <key> - הצגת ה-TTL של Key" -ForegroundColor White
    Write-Host "4. INFO - מידע כללי על Redis" -ForegroundColor White
    Write-Host "5. DBSIZE - ספירת Keys" -ForegroundColor White
    Write-Host "6. FLUSHALL - הסרת כל ה-Keys" -ForegroundColor White
    Write-Host "7. CLI - כניסה ל-Interactive Mode" -ForegroundColor White
    Write-Host "8. Exit" -ForegroundColor White
    Write-Host "============================================" -ForegroundColor Cyan
}

function Main {
    if (-not [string]::IsNullOrWhiteSpace($Command)) {
        # Execute command from parameter
        switch ($Command.ToUpper()) {
            "KEYS" { Show-AllKeys }
            "GET" { Show-KeyValue (Read-Host "הקלד את שם ה-Key") }
            "TTL" { Show-KeyTTL (Read-Host "הקלד את שם ה-Key") }
            "INFO" { Show-RedisInfo }
            "DBSIZE" { Show-KeyCount }
            "CLI" { Connect-Redis }
            default {
                Write-Host "❌ Command לא מוכר" -ForegroundColor Red
            }
        }
    }
    else {
        # Interactive menu
        $choice = ""
        while ($choice -ne "8") {
            Show-Menu
            $choice = Read-Host "בחר אפשרות"
            
            switch ($choice) {
                "1" { Show-AllKeys }
                "2" { Show-KeyValue (Read-Host "הקלד את שם ה-Key") }
                "3" { Show-KeyTTL (Read-Host "הקלד את שם ה-Key") }
                "4" { Show-RedisInfo }
                "5" { Show-KeyCount }
                "6" { Clear-AllKeys }
                "7" { Connect-Redis }
                "8" { 
                    Write-Host "👋 שלום!" -ForegroundColor Green
                    break
                }
                default { Write-Host "❌ בחירה לא תקינה" -ForegroundColor Red }
            }
        }
    }
}

Main
