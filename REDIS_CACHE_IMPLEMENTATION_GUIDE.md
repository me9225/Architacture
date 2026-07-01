# 🔴 מדריך בדיקה וטיפול בעדכון Redis Cache

## ✅ מה שכבר נעשה:

1. **Docker Compose Setup** - Redis עם password בדוקר
2. **CacheService** - שירות מרכזי לניהול cache עם TTL
3. **GiftService** - מטמיע caching ו-invalidation
4. **BasketService** - עדכן עם caching ו-invalidation
5. **Program.cs** - רישום ב-DI וקונפיגורציה

---

## 📌 שלב 1: הפעלת Docker Compose

### צעד 1.1: הפעל Docker Desktop
פתח את Docker Desktop והוודא שהוא פועל.

### צעד 1.2: הפעל את Docker Compose
```powershell
docker-compose up -d
```

### צעד 1.3: בדוק ש-Redis פועל
```powershell
docker ps
```
אתה אמור לראות `bsd_redis` רץ.

---

## 📌 שלב 2: בדיקה של Redis מ-Command Line

### צעד 2.1: כניסה ל-Redis Container
```powershell
docker exec -it bsd_redis redis-cli -a YourRedisPassword123
```

### צעד 2.2: פקודות בדיקה שימושיות

```redis
# לראות את כל ה-keys
KEYS *

# לראות את ערך key ספציפי
GET gift_1

# לראות את ה-TTL של key
TTL gift_1

# לראות מידע כללי על Redis
INFO

# לראות כמה keys קיימים
DBSIZE

# מחק כל ה-keys (זהיר!)
FLUSHALL

# צא מ-Redis CLI
EXIT
```

---

## 📌 שלב 3: בדיקה של Implementation בקוד

### צעד 3.1: בדוק את appsettings.Development.json
זה בעדכון טוב עם Redis Connection String:
```json
"Redis": {
  "Host": "redis",
  "Port": "6379",
  "Password": "YourRedisPassword123"
}
```

### צעד 3.2: בדוק את Program.cs
הרישום צריך להיות שם:
```csharp
var redis = ConnectionMultiplexer.Connect(redisConnectionString);
builder.Services.AddSingleton<IConnectionMultiplexer>(redis);
builder.Services.AddScoped<ICacheService, CacheService>();
```

---

## 📌 שלב 4: בדיקה של Cache TTL בפעולה

### צעד 4.1: הפעל את ה-API
```powershell
dotnet run
```

### צעד 4.2: בצע GET Request ל-Gift
```
GET http://localhost:5000/api/gift/1
```

### צעד 4.3: בדוק Redis עכשיו
```powershell
docker exec -it bsd_redis redis-cli -a YourRedisPassword123
KEYS *
GET gift_1
TTL gift_1
```

אתה אמור לראות:
- Key בשם `gift_1` קיים
- ערך JSON עם הפרטים של ה-gift
- TTL שקטן מ-3600 (כי זה בעדכון ממשי)

### צעד 4.4: המתן לפקיעת ה-TTL
```powershell
# המתן 1 שעה, או שנה את TTL לערך קטן יותר לבדיקה
# אפשר לשנות את appsettings.Development.json:
# "DefaultTTL": "60" (60 שניות במקום 3600)

# בדוק שוב אחרי הזמן
TTL gift_1
# צריך להיות -2 (כלומר לא קיים)
```

---

## 📌 שלב 5: Cache Invalidation בשינויים

### צעד 5.1: בדוק את CreateNewBasket
```
POST http://localhost:5000/api/basket
{
  "userId": 1,
  "giftId": 1
}
```

### צעד 5.2: בדוק Redis
```redis
KEYS user_baskets_1
```
- כדי כי התוצאה עשויה להיות ריקה (ה-cache בוטל אחרי הכנסה)

### צעד 5.3: בדוק את GetAllMyBasket
```
GET http://localhost:5000/api/basket/user/1
```

### צעד 5.4: בדוק Redis שוב
```redis
KEYS user_baskets_1
```
- עכשיו צריך להיות קיים `user_baskets_1` key

---

## 📌 שלב 6: טיפול ב-Services נוספות (אופציונלי)

אם תרצה להוסיף Redis גם ל-CategoryService, DonorService וכו', בדוק את הדוקומנטציה הבאה.

### CategoryService
```csharp
// אם יש הרבה קטגוריות, זה כדאי לקש קטן
// אבל אם יש מעט, אפשר גם בלעדיו

// פעולות Get: הוסף cache
// פעולות Create/Update/Delete: בטל cache
```

### DonorService
דומה ל-CategoryService.

---

## 🔧 שכחת משהו? בדוק את ה-Log Files

```powershell
# בדוק את ה-logs
Get-Content BsdFinalProject\logs\log.txt -Tail 50
```

---

## 📝 סמ"ה תמצא ב-redis-helper.ps1

יש לך דוקומנטציה מקיפה ב:
- `redis-helper.ps1` - PowerShell helper script
- `REDIS_TESTING_GUIDE.md` - מדריך בדיקה מקיף
- `REDIS_SUMMARY.md` - סיכום של כל הדברים

---

## ✅ בדיקה סופית

לפני שאתה מסכים שכל זה עובד:

1. ✅ Docker Compose רץ ו-Redis accessible
2. ✅ API משיקה בהצלחה
3. ✅ GET Requests מוזנים לפני מה-cache
4. ✅ TTL עובד (keys נמחקים אחרי הזמן)
5. ✅ Cache invalidation עובד (שינויים מבטלים cache)
6. ✅ Logs מראים ש-cache עובד

---

## 🎯 שלב הבא

1. בדוק ש-Redis פועל בדוקר
2. הפעל את ה-API
3. בצע בדיקות כמו למעלה
4. אם הכל עובד, תוכל לבקש ממני עזרה עם שדים נוספים!

