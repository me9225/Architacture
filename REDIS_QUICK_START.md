# ⚡ Quick Start Guide - Redis Cache

## 🚀 התחלה מהירה (5 דקות)

### צעד 1: הפעל Docker Compose
```powershell
cd C:\גילי-מסלול\ArcitactureProject2
docker-compose up -d
```

**בדיקה:**
```powershell
docker ps
```
ראה `bsd_redis` ו-`bsd_sqlserver`?

### צעד 2: הפעל את ה-API
```powershell
cd BsdFinalProject
dotnet run
```

API רץ על: `http://localhost:5000`

### צעד 3: בצע Request
```powershell
# דרך Swagger
http://localhost:5000/swagger

# או דרך curl
curl -X GET "http://localhost:5000/api/gift/1"
```

### צעד 4: בדוק Redis
```powershell
docker exec -it bsd_redis redis-cli -a YourRedisPassword123
KEYS *
GET gift_1
TTL gift_1
EXIT
```

---

## 📝 הערות חשובות

### Redis Keys Pattern:
- `gift_{id}` - single gift
- `all_gifts` - all gifts
- `user_baskets_{id}` - user's basket
- `user_cards_{id}` - user's cards
- `category_{id}` - single category
- `donor_{id}` - single donor
- וכו'

### Default TTL: 3600 seconds (1 hour)

לשינוי, עדכן את `appsettings.Development.json`:
```json
"Cache": {
  "DefaultTTL": "60"  // 60 seconds
}
```

---

## 🔥 Common Commands

### ראה כל ה-Keys:
```powershell
docker exec bsd_redis redis-cli -a YourRedisPassword123 KEYS "*"
```

### ראה ערך ספציפי:
```powershell
docker exec bsd_redis redis-cli -a YourRedisPassword123 GET gift_1
```

### מחק Key:
```powershell
docker exec bsd_redis redis-cli -a YourRedisPassword123 DEL gift_1
```

### מחק הכל:
```powershell
docker exec bsd_redis redis-cli -a YourRedisPassword123 FLUSHALL
```

---

## 🐛 Troubleshooting

| בעיה | פתרון |
|------|--------|
| Redis not running | `docker-compose up -d` |
| Connection refused | בדוק password בappsettings.Development.json |
| API slow | בדוק `DBSIZE` בRedis - אולי יש הרבה keys |
| Cache not working | בדוק logs בـ `BsdFinalProject\logs\log.txt` |

---

## 📊 Monitoring

### View cache size:
```powershell
docker exec bsd_redis redis-cli -a YourRedisPassword123 DBSIZE
```

### View memory:
```powershell
docker exec bsd_redis redis-cli -a YourRedisPassword123 INFO memory
```

### View all stats:
```powershell
docker exec bsd_redis redis-cli -a YourRedisPassword123 INFO
```

---

## 🎯 Test Scenarios

### Scenario 1: Cache Hit
1. GET /api/gift/1 → Database query
2. GET /api/gift/1 → Redis cache
3. logs show "retrieved from cache"

### Scenario 2: Cache Invalidation
1. GET /api/gift/1 → Cached
2. PUT /api/gift/1 → Update
3. Cache cleared
4. GET /api/gift/1 → Database query again

### Scenario 3: TTL Expiration
1. GET /api/gift/1 → Cached with 1 hour TTL
2. Wait 1 hour
3. GET /api/gift/1 → Key expired, database query

---

## 💡 Pro Tips

1. **Shorter TTL for testing:**
   ```json
   "DefaultTTL": "10"  // 10 seconds
   ```

2. **Monitor via logs:**
   ```powershell
   Get-Content BsdFinalProject\logs\log.txt -Tail 20 -Wait
   ```

3. **Interactive Redis CLI:**
   ```powershell
   docker exec -it bsd_redis redis-cli -a YourRedisPassword123
   # Then type commands: KEYS *, GET key, etc.
   ```

---

## ✅ Success Indicators

- ✅ Docker shows `bsd_redis` running
- ✅ API starts without errors
- ✅ Redis has keys after GET requests
- ✅ Logs show "retrieved from cache"
- ✅ Keys expire after TTL

---

**That's it! Happy caching! 🚀**

For more details, see:
- `REDIS_COMPLETE_TESTING_GUIDE.md`
- `REDIS_IMPLEMENTATION_SUMMARY.md`
