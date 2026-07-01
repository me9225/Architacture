# Redis Cache Implementation - Testing Guide

## 📋 שלבי הבדיקה

### **שלב 1: הפעלת Docker Compose עם Redis**

```powershell
# עבור לתיקיית הפרויקט
cd C:\גילי-מסלול\ArcitactureProject2

# הפעל את כל ה-containers
docker-compose up -d

# בדוק שהכל עובד
docker-compose ps
```

**צפוי לראות:**
```
NAME          STATUS
bsd_sqlserver Up (healthy)
bsd_redis     Up (healthy)
bsd_api       Up
```

---

### **שלב 2: בדיקה בـ Docker Desktop**

1. פתח את **Docker Desktop**
2. עבור ל-**Containers**
3. בדוק ש-3 containers רצים:
   - `bsd_sqlserver`
   - `bsd_redis`
   - `bsd_api`

---

### **שלב 3: בדיקת API endpoints**

#### **קבל את כל ה-Gifts (עם Cache)**

1. בקש ראשון - מן ה-Database:
```bash
curl http://localhost:5000/api/gifts
```
- זה ייקח קצת יותר זמן (קריאה מ-DB)

2. בקש שני - מן ה-Cache:
```bash
curl http://localhost:5000/api/gifts
```
- זה יהיה הרבה יותר מהיר!

#### **קבל Gift לפי ID**
```bash
curl http://localhost:5000/api/gifts/1
```

---

### **שלב 4: גישה ל-Redis Container**

#### **שיטה 1: התחבר לפי PowerShell**

```powershell
# התחבר ל-Redis container
docker exec -it bsd_redis redis-cli -a YourRedisPassword123

# כעת אתה בתוך Redis CLI
```

#### **שיטה 2: שיטה קצרה - בעזרת Bash**

```powershell
# ביצע פקודה במישרין ב-Redis
docker exec -it bsd_redis redis-cli -a YourRedisPassword123 KEYS "*"
```

---

### **שלב 5: Redis CLI - פקודות שימושיות**

**כשאתה בתוך Redis CLI:**

#### **1. ראה את כל ה-Keys שקיימות**
```redis
KEYS *
```
**צפוי:**
```
1) "gift_1"
2) "all_gifts"
3) "gifts_category_5"
```

#### **2. ראה תוכן של Key ספציפי**
```redis
GET gift_1
```
**צפוי:** JSON של ה-Gift

#### **3. ראה TTL (Time To Live) של Key**
```redis
TTL gift_1
```
**צפוי:** מספר שניות (למשל 3600 ל-1 שעה)

#### **4. ראה את כל המידע על Key**
```redis
GETEX gift_1
```

#### **5. מחק Key ספציפי**
```redis
DEL gift_1
```

#### **6. מחק את כל ה-Keys**
```redis
FLUSHALL
```

#### **7. ראה סטטיסטיקה**
```redis
INFO stats
INFO memory
```

---

### **שלב 6: בדיקת Cache Invalidation**

#### **בדיקה 1: עדכון Gift צריך להוריד Cache**

1. **קבל Gift #1** (יישמר בـ Cache):
```bash
curl http://localhost:5000/api/gifts/1
```

2. **בדוק את Redis**:
```powershell
docker exec -it bsd_redis redis-cli -a YourRedisPassword123 KEYS "gift_1"
```
**צפוי:** יהיה שם `gift_1`

3. **עדכן את Gift #1**:
```bash
curl -X PUT http://localhost:5000/api/gifts/1 \
  -H "Content-Type: application/json" \
  -d '{"id":1,"name":"Updated Gift","description":"...","cost":100,"picture":"...","categoryId":1,"donorId":1}'
```

4. **בדוק Redis שוב**:
```powershell
docker exec -it bsd_redis redis-cli -a YourRedisPassword123 KEYS "gift_1"
```
**צפוי:** `gift_1` לא יהיה בשום - זה נמחק (Cache Invalidation!)

---

### **שלב 7: בדיקת TTL Expiration**

1. **קבל את כל ה-Gifts** (יישמרו בـ Cache 1 שעה):
```bash
curl http://localhost:5000/api/gifts
```

2. **בדוק את TTL**:
```powershell
docker exec -it bsd_redis redis-cli -a YourRedisPassword123 TTL all_gifts
```
**צפוי:** כמעט 3600 שניות

3. **חכה ועקוב אחרי TTL**:
```powershell
# כל 30 שניות בדוק
docker exec -it bsd_redis redis-cli -a YourRedisPassword123 TTL all_gifts
```
ה-מספר ירד בהדרגה

4. **כשה-TTL יעבור ל-0 (או -2):**
- ה-Key יימחק אוטומטית מ-Redis
- בקשת הבאה ל-`/api/gifts` תקרא מן ה-DB שוב

---

### **שלב 8: צפיית Logs**

#### **צפה ב-API Logs עם Cache הודעות**

```powershell
docker logs -f bsd_api
```

**צפוי לראות הודעות כמו:**
```
[10:30:45 INF] Gift 1 retrieved from cache
[10:30:46 INF] All gifts stored in cache
[10:31:20 INF] Updated gift 1 and invalidated cache
```

---

## 🧪 Automated Test Script

אם רוצים לבדוק הכל אוטומטית, אפשר להריץ את הפקודה הזו:

```powershell
# בדוק שכל Containers רצים בריאים
docker-compose ps

# התחבר ל-Redis ובדוק
docker exec bsd_redis redis-cli -a YourRedisPassword123 PING

# קבל קצה API
curl -s http://localhost:5000/api/gifts | jq .

# בדוק Redis keys
docker exec bsd_redis redis-cli -a YourRedisPassword123 KEYS "*"

# בדוק TTL
docker exec bsd_redis redis-cli -a YourRedisPassword123 TTL all_gifts
```

---

## 🔍 Debugging Redis Connection

אם יש בעיות בחיבור:

```powershell
# בדוק שה-password נכון
docker exec bsd_redis redis-cli -a WrongPassword PING
# צפוי לקבל: (error) WRONGPASS invalid username-password pair

# בדוק את Connection String
docker logs bsd_api | grep -i redis

# בדוק Port
docker port bsd_redis
```

---

## 📊 Performance Comparison

### **זמן בקריאות עם vs בלי Cache**

**1. בקריאה ראשונה (No Cache):**
```
⏱️ בערך 150-300ms (קריאה מ-DB)
```

**2. בקריאה שנייה (With Cache):**
```
⏱️ בערך 5-15ms (קריאה מ-Redis)
```

**הצבה:** Cache מעביר בערך **10-20x מהיר יותר!** 🚀

---

## 📝 Notes

- TTL default = 3600 שניות (1 שעה) - אפשר לשנות בـ `appsettings.json`
- Cache עובד עבור: `GetById`, `GetAll`, `GetByCategory`, `GetByCostRange`
- Cache נמחק אוטומטית כשעדכנו/מחקנו/יצרנו Gift
- Redis רץ ב-Port 6379 (ברירת מחדל)
- Password: `YourRedisPassword123` (צריך לשנות בפרודקשן!)

---

## ✅ בדיקה סופית

כדי לבדוק שהכל עובד נכון:

```powershell
# 1. Containers רצים
docker-compose ps

# 2. Redis בריא
docker exec bsd_redis redis-cli -a YourRedisPassword123 PING
# צפוי: PONG

# 3. API רץ
curl http://localhost:5000/swagger
# צפוי: Swagger UI

# 4. Cache עובד
curl http://localhost:5000/api/gifts
docker exec bsd_redis redis-cli -a YourRedisPassword123 KEYS "all_gifts"
# צפוי: קיים key

# 5. TTL עובד
docker exec bsd_redis redis-cli -a YourRedisPassword123 TTL all_gifts
# צפוי: מספר חיובי
```

🎉 **אם הכל הדפיס בהצלחה - Redis Cache עובד תקין!**
