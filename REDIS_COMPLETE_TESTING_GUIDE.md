# 🔴 מדריך בדיקה מקיף של Redis Cache Implementation

## ✅ סוכם: מה שכבר הושלם

### 1️⃣ Redis Setup בDocker
- ✅ `docker-compose.yml` - Redis עם password
- ✅ Health checks וnetwork ב-docker

### 2️⃣ Code Configuration
- ✅ `Program.cs` - Redis registration ב-DI
- ✅ `appsettings.json` - Redis connection strings
- ✅ `appsettings.Development.json` - Docker config

### 3️⃣ Cache Service
- ✅ `CacheService.cs` - Wrapper ל-StackExchange.Redis
- ✅ TTL support
- ✅ Pattern deletion support

### 4️⃣ Services עם Cache
✅ **GiftService** - Cache עם invalidation
✅ **BasketService** - Cache עם invalidation
✅ **CategoryService** - Cache עם invalidation
✅ **DonorService** - Cache עם invalidation
✅ **CardService** - Cache עם invalidation
✅ **WinnerService** - Cache עם invalidation

---

## 🚀 שלב 1: הפעל את כל זה

### צעד 1: הפעל Docker Desktop
- פתח את Docker Desktop
- עדכן שהוא רץ (חכה 30 שניות)

### צעד 2: הפעל את docker-compose
```powershell
cd C:\גילי-מסלול\ArcitactureProject2
docker-compose up -d
```

**בדיקה:**
```powershell
docker ps
```
אתה אמור לראות:
- `bsd_sqlserver` (SQL Server)
- `bsd_redis` (Redis)

### צעד 3: בדוק ש-Redis זמין
```powershell
docker exec bsd_redis redis-cli -a YourRedisPassword123 ping
```
צריך להדפיס: `PONG`

---

## 🧪 שלב 2: בדיקה ידנית של Redis

### דרך 1: השתמש בPowerShell helper (אופציונלי)
```powershell
.\redis-testing-helper.ps1
```

### דרך 2: ישירות בـ Command Line

**כניסה ל-Redis CLI:**
```powershell
docker exec -it bsd_redis redis-cli -a YourRedisPassword123
```

**פקודות שימושיות:**
```redis
# ראה את כל ה-keys
KEYS *

# ספירת keys
DBSIZE

# ראה ערך של key
GET gift_1

# ראה TTL של key
TTL gift_1

# ראה מידע כללי
INFO

# מחק key ספציפי
DEL gift_1

# מחק את כל ה-keys
FLUSHALL

# צא
EXIT
```

---

## 🔍 שלב 3: בדיקה של API עם Cache

### צעד 1: הפעל את ה-API
```powershell
cd BsdFinalProject
dotnet run
```

### צעד 2: בצע GET Request ל-Gift

**Option A: Swagger**
```
http://localhost:5000/swagger
```
בחר GET /api/gift/{id} וניסה עם id=1

**Option B: curl**
```powershell
curl -X GET "http://localhost:5000/api/gift/1" -H "accept: application/json"
```

**Option C: PowerShell**
```powershell
Invoke-WebRequest -Uri "http://localhost:5000/api/gift/1" -Method Get
```

### צעד 3: בדוק ב-Redis שה-cache עובד

```powershell
# בטרמינל שני, בדוק Redis
docker exec -it bsd_redis redis-cli -a YourRedisPassword123

# בקע בתוך Redis:
KEYS *
```

אתה אמור לראות keys כמו:
- `gift_1` (מהרequests)
- `all_gifts` (אם ביצעת GET All)

### צעד 4: בדוק את ה-TTL

```redis
TTL gift_1
```
צריך להיות מספר בין 0 ל-3600

---

## 🔄 שלב 4: בדיקה של Cache Invalidation

### צעד 1: בצע DELETE/UPDATE request
```powershell
# PUT (update)
$body = @{
    id = 1
    name = "Updated Gift"
    description = "New description"
    cost = 100
    categoryId = 1
    donorId = 1
} | ConvertTo-Json

Invoke-WebRequest -Uri "http://localhost:5000/api/gift/1" `
    -Method Put `
    -ContentType "application/json" `
    -Body $body
```

### צעד 2: בדוק ב-Redis שה-cache בוטל
```redis
KEYS gift_1
# אמור להיות ריק
```

### צעד 3: בצע GET חדש לראות אם הקוד שולף מה-database
```powershell
curl -X GET "http://localhost:5000/api/gift/1" -H "accept: application/json"
```

---

## 📊 שלב 5: בדיקה של Basket עם Cache

### צעד 1: בצע GET Basket
```powershell
Invoke-WebRequest -Uri "http://localhost:5000/api/basket/user/1" -Method Get
```

### צעד 2: בדוק בRedis
```redis
KEYS user_baskets_*
```

### צעד 3: צור basket חדש
```powershell
$body = @{
    userId = 1
    giftId = 1
} | ConvertTo-Json

Invoke-WebRequest -Uri "http://localhost:5000/api/basket" `
    -Method Post `
    -ContentType "application/json" `
    -Body $body
```

### צעד 4: בדוק שה-cache בוטל
```redis
KEYS user_baskets_1
# אמור להיות ריק (בוטל אחרי ה-create)
```

---

## ⏰ שלב 6: בדיקה של TTL

### צעד 1: שנה את ה-TTL לערך קטן
עדכן את `appsettings.Development.json`:
```json
"Cache": {
  "DefaultTTL": "10"  // 10 שניות
}
```

### צעד 2: הפעל את ה-API מחדש
```powershell
dotnet run
```

### צעד 3: בצע GET request
```powershell
Invoke-WebRequest -Uri "http://localhost:5000/api/gift/1" -Method Get
```

### צעד 4: בדוק את TTL
```redis
TTL gift_1
# אמור להיות בין 0 ל-10
```

### צעד 5: המתן 10 שניות
```powershell
Start-Sleep -Seconds 11
```

### צעד 6: בדוק שה-key נמחק
```redis
TTL gift_1
# אמור להיות -2 (key לא קיים)
```

---

## 📝 שלב 7: בדיקה של Logs

### כדי לראות את ה-logs של ה-cache:
```powershell
Get-Content "BsdFinalProject\logs\log.txt" -Tail 50
```

אתה אמור לראות entries כמו:
```
[00:00:00 INF] Gift 1 retrieved from cache
[00:00:01 INF] Gift 1 stored in cache
[00:00:02 INF] Updated gift 1 and invalidated cache
```

---

## 🎯 בדיקה סופית

### Checklist:
- ✅ Docker Compose רץ ש-Redis זמין
- ✅ API משיקה בהצלחה
- ✅ GET Requests כיתובות ל-cache כמו שרואים ב-logs
- ✅ Redis מכיל keys כמו `gift_1`, `all_gifts` וכו'
- ✅ TTL כנון מנוהל (keys נמחקים אחרי הזמן)
- ✅ Invalidation עובד (שינויים מבטלים cache)
- ✅ Logs מראים שהcache פעיל

---

## 🐛 Troubleshooting

### בעיה: Redis לא מהרד
```powershell
docker-compose down
docker-compose up -d
```

### בעיה: Connection refused
בדוק את:
1. Docker Desktop פתוח?
2. `docker ps` מראה את Redis?
3. password נכון בـ appsettings.Development.json?

### בעיה: Cache לא עובד
בדוק את הlogfiles בـ `BsdFinalProject\logs\log.txt`

### בעיה: Serialization errors
בדוק ש-objects JSON-able (אין circular references)

---

## 📚 תיעוד נוסף

- `REDIS_IMPLEMENTATION.md` - עקיפה טכנית
- `REDIS_SUMMARY.md` - סיכום תכונות
- `redis-helper.ps1` - Helper script ישן
- `redis-testing-helper.ps1` - Helper script חדש

---

## ✅ הכל סדור!

לאחר בדיקה מוצלחת, הקוד שלך מוכן לצור הקדמי עם:
- ✅ Redis Cache עבור קריאות נתונים
- ✅ TTL אוטומטי
- ✅ Cache invalidation בעדכונים
- ✅ Performance boost משמעותי

בהצלחה! 🚀
