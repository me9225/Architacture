# תכנון חלוקה למיקרו סרביסים - BsdFinalProject

**סטטוס:** 📝 תכנון בעלייה - טרם מומש

## 🎯 סיכום ביצוע

**מספר סרביסים:** 6 סרביסים + API Gateway = 7 יחידות

| Microservice | תיאור | Entities | Database | תלויויות |
|---|---|---|---|---|
| 🧑‍💼 **User Service** | משתמשים ואימות | User, Role, Profile | PostgreSQL | Notification |
| 📚 **Catalog Service** | מתנות וקטגוריות | Gift, Category, Image | PostgreSQL + Redis | - (independent) |
| 🛍️ **Shopping Service** | סל קניות + הזמנות + תשלומים | Basket, Order, Card, Transaction | PostgreSQL + Redis | Catalog, User, Notification |
| 🤝 **Donor Service** | תורמים + זוכים + הגרלות | Donor, Winner, Drawing, Prize | PostgreSQL | User, Catalog, Notification |
| 📬 **Notification Service** | מיילים, SMS, הודעות | Notification, Template, Log, Preferences | PostgreSQL (optional) | RabbitMQ, External Services |
| 📊 **Analytics Service** | דוחות וניתוח (Optional) | Metrics, Reports, Behavior | MongoDB/ClickHouse | Event Bus |
| 🚪 **API Gateway** | נתב ראשי | - | - | All Services |

---

## תיאור כללי

מסמך זה מתאר תכנית להתפתחות ארכיטקטורת המערכת מ-Monolithic לעבור **Microservices Architecture**. 
זה יאפשר סקלביליות טובה יותר, גמישות בפיתוח, וקלות בתחזוקה של כל קומפוננטה בנפרד.

### 🔄 שילוב דומיינים הגיוני:
```
Monolithic:
├── Basket ──┐
├── Order   ─┼─→ Shopping Service 🛍️
├── Payment ─┤
└── Card ───┘

├── Gift ───┐
└── Category ─→ Catalog Service 📚

├── Donor ──┐
├── Winner ─┼─→ Donor Service 🤝
└── Drawing ┘
```

---

## 1. מצב הנוכחי (Monolithic)

### המבנה הקיים:
```
BsdFinalProject (Monolith)
├── Controllers/           # כל ה-API endpoints
│   ├── BasketsController
│   ├── GiftsController
│   ├── UsersController
│   ├── DonorsController
│   ├── WinnersController
│   ├── CardsController
│   ├── CategoriesController
│   └── ManagersController
├── Repositories/          # כל גישה לנתונים
├── Services/              # כל הלוגיקה העסקית
├── Models/                # כל הישויות
├── DTOs/                  # כל ה-Data Transfer Objects
└── Data/
    └── SaleContext        # בסיס נתונים אחד

```

### בעיות הנוכחיות:
- 🔴 **חוסר גמישות בפיתוח**: צוות אחד מעדכן את כולם
- 🔴 **סקלביליות מוגבלת**: יש לשדרג את כל המערכת בבת אחת
- 🔴 **תלות הדדית**: שינוי בדומיין אחד משפיע על האחרים
- 🔴 **חסר אוטונומיה**: כל סרביס תלוי בשאר הסרוויסים

---

## 2. ארכיטקטורת מיקרו סרביסים המוצעת

### 2.1 חלוקה לפי דומיין (Domain-Driven Design)

```
microservices/
├── user-service/                        # ניהול משתמשים וחשבונות
├── catalog-service/                     # מתנות וקטגוריות
├── shopping-service/                    # סל קניות, הזמנות, תשלומים
├── donor-service/                       # תורמים, זוכים והגרלות
├── notification-service/                # הודעות ועדכונים (async)
├── analytics-service/                   # דוחות וניתוח (optional)
└── api-gateway/                         # שער כניסה מרכזי
```

**שילוב הגיוני:**
- ✅ **Basket + Order + Payment** → Shopping Service (זרימה קנייה אחת)
- ✅ **Gift + Category** → Catalog Service (ניהול מוצרים)
- ✅ **Donor + Winner** → Donor Service (בדומיין התרומה)

---

### 2.2 פירוט כל סרביס

#### **1️⃣ User Service** 🧑‍💼
**אחראים:** משתמשים, אימות, הרשאות

**Entities:**
- User
- Role/Permission
- UserProfile

**API Endpoints:**
```
GET    /api/users/{id}
GET    /api/users/profile
POST   /api/users/register
PUT    /api/users/{id}
DELETE /api/users/{id}
POST   /api/auth/login
POST   /api/auth/logout
POST   /api/auth/refresh-token
```

**Database:** `UserDb` (PostgreSQL)

**Dependencies:** 
- Notification Service (להודעות רישום/אימות)

---

#### **2️⃣ Catalog Service** 📚
**אחראים:** מתנות, קטגוריות, תיאוריהן, תמונות

**Entities:**
- Gift (Product)
- Category
- GiftImage
- GiftReview

**API Endpoints:**
```
GET    /api/products
GET    /api/products/{id}
GET    /api/products/category/{categoryId}
GET    /api/categories
POST   /api/products (admin only)
PUT    /api/products/{id} (admin only)
DELETE /api/products/{id} (admin only)
POST   /api/categories (admin only)
PUT    /api/categories/{id} (admin only)
```

**Database:** `CatalogDb` (PostgreSQL)

**Dependencies:**
- None (עצמאי לחלוטין)

**Notes:**
- קריאה בלבד לעיתים קרובות → ניתן להוסיף Caching (Redis)
- יכול להיות CDN עבור תמונות

---

#### **3️⃣ Shopping Service** 🛍️ (ממוזג)
**אחראים:** סל קניות, הזמנות, תשלומים

**Entities:**
- Basket / Cart
- BasketItem
- Order
- OrderItem
- OrderStatus
- Card (Payment Method)
- Transaction
- Payment

**API Endpoints:**
```
# Basket Operations
GET    /api/cart/my-cart
POST   /api/cart/add-item
PUT    /api/cart/update-item/{itemId}
DELETE /api/cart/remove-item/{itemId}
DELETE /api/cart/clear

# Order Operations
POST   /api/orders (checkout)
GET    /api/orders/my-orders
GET    /api/orders/{id}
PUT    /api/orders/{id}/cancel

# Payment Operations
POST   /api/payments/process
GET    /api/payments/invoice/{id}
POST   /api/cards/add
GET    /api/cards/my-cards
DELETE /api/cards/{id}
```

**Database:** `ShoppingDb` (PostgreSQL - orders, cards) + Redis (cart cache)

**Dependencies:**
- Catalog Service (לחיפוש פרטי מתנה)
- User Service (לאימות משתמש)
- Notification Service (לעדכונים ודוחות)
- External: Payment Gateway (Stripe, PayPal)

**Notes:**
- זה **ליבה של המערכת** - סרביס קריטי
- סל קניות בזיכרון (Redis) לביצועים
- הזמנות בDB קבוע
- Saga Pattern עבור תשלום → הזמנה

---

#### **4️⃣ Donor Service** 🤝 (ממוזג)
**אחראים:** תורמים, זוכים, הגרלות, חלוקה

**Entities:**
- Donor
- DonorProfile
- Donation / Contribution
- Winner
- Drawing / Lottery
- Prize
- DonorStats

**API Endpoints:**
```
# Donor Management
POST   /api/donors/register
GET    /api/donors/{id}
GET    /api/donors/my-profile
PUT    /api/donors/{id}

# Donations & Tracking
GET    /api/donations/my-donations
GET    /api/donations/stats
GET    /api/donations/{id}

# Winners & Lottery
GET    /api/winners
GET    /api/winners/{id}
GET    /api/winners/my-prizes
POST   /api/drawings/run (admin only)
GET    /api/drawings/{id}
```

**Database:** `DonorDb` (PostgreSQL)

**Dependencies:**
- User Service (לאימות)
- Catalog Service (לנתוני מתנות)
- Notification Service (להודעות ניצחון)
- Analytics Service (לסטטיסטיקות)

**Notes:**
- Donor יכול להיות גם User רגיל שרוצה להתרום
- Winner = זוכה בהגרלה של מתנה שתרם

---

#### **5️⃣ Notification Service** 📬
**אחראים:** עדכונים, מיילים, SMS, הודעות

**Entities:**
- Notification
- EmailTemplate
- NotificationLog
- UserPreferences

**API Endpoints:**
```
POST   /api/notifications/send-email
POST   /api/notifications/send-sms
GET    /api/notifications/my-notifications
PUT    /api/notifications/mark-read/{id}
GET    /api/notifications/preferences
PUT    /api/notifications/preferences
```

**Message Queue:** RabbitMQ / Azure Service Bus (עקבי עם אחרים)

**Dependencies:**
- User Service (לכתובות)
- External: Email Service (SendGrid, Mailgun)
- External: SMS Service (Twilio)

**Notes:**
- רובו **async** - מקבל אירועים מסרביסים אחרים
- הוא publisher, לא subscriber ישירות
- Queue-based עבור reliability

---

#### **6️⃣ Analytics Service** 📊 (Optional)
**אחראים:** דוחות, סטטיסטיקות, ניתוח

**Entities:**
- Report
- Metric
- UserBehavior
- SystemStats
- SalesData

**API Endpoints:**
```
GET    /api/analytics/dashboard
GET    /api/analytics/sales-report
GET    /api/analytics/user-stats
GET    /api/analytics/top-gifts
GET    /api/analytics/donor-stats
GET    /api/analytics/monthly-summary
```

**Database:** `AnalyticsDb` (MongoDB / ClickHouse - read optimized)

**Dependencies:**
- Shopping Service (לנתונים על הזמנות)
- Catalog Service (לנתונים על מתנות)
- User Service (לנתונים על משתמשים)
- Donor Service (לסטטיסטיקות תרומה)
- Event Bus (מאזין לכל האירועים)

**Notes:**
- **Read-only** עבור consumers
- מקבל ממסד נתונים לא מנורמל (denormalized)
- יכול להיות delayed (eventual consistency)

---

#### **7️⃣ API Gateway** 🚪
**אחראים:** נתב בקשות, אימות, rate limiting

**Responsibilities:**
- Route to appropriate service
- Authentication & Authorization (JWT validation)
- Request/Response transformation
- Rate limiting & throttling
- Request logging
- Error handling

**Technology:**
- Ocelot / Kong

**Configuration:**
```yaml
routes:
  - users: http://user-service:5001
  - catalog: http://catalog-service:5002
  - shopping: http://shopping-service:5003
  - donor: http://donor-service:5004
  - notifications: http://notification-service:5005
  - analytics: http://analytics-service:5006
```

---

## 3. Communication Patterns

### 3.1 Synchronous Communication (REST/gRPC)

```
Scenario: Checkout Process
┌────────────────────────────────────────────────────────────────┐
│ 1. Client → API Gateway → Shopping Service                   │
│ 2. Shopping Service → Catalog Service (validate items price) │
│ 3. Shopping Service → User Service (validate user)           │
│ 4. Shopping Service → External Payment Gateway               │
│ 5. Shopping Service → Notification Service (send confirmation)
└────────────────────────────────────────────────────────────────┘
```

### 3.2 Asynchronous Communication (Message Queue - RabbitMQ)

```
Scenario: Order Placed Event
┌────────────────────────────────────────────────────────────────┐
│ 1. Shopping Service: publishes "OrderCreated" event          │
│ 2. Message Queue (RabbitMQ)                                   │
│ 3. Multiple subscribers process independently:              │
│    - Notification Service: send email/SMS                    │
│    - Analytics Service: record order metrics                 │
│    - Donor Service: update lottery odds (if donated gifts)   │
└────────────────────────────────────────────────────────────────┘
```

### 3.3 Distributed Transactions (Saga Pattern)

```
Pattern: Checkout → Payment → Order Creation Saga

State 1: ORDER_PENDING
├─> Shopping Service creates order (status: PENDING)
└─> publishes: CheckoutInitiated

State 2: PAYMENT_PROCESSING
├─> Shopping Service calls Payment Gateway
└─> if success: publishes PaymentSucceeded
    if fail: publishes PaymentFailed → Order cancelled

State 3: ORDER_CONFIRMED / ORDER_FAILED
└─> Shopping Service updates order status
    and publishes OrderConfirmed/OrderFailed
```

---

## 4. Data Management Strategy

### 4.1 Database per Service (Data Independence)

```
User Service
    └─ UserDb (PostgreSQL)
        ├─ Users
        ├─ Roles/Permissions
        └─ UserProfile

Catalog Service
    └─ CatalogDb (PostgreSQL)
        ├─ Gifts/Products
        ├─ Categories
        └─ GiftImages

Shopping Service
    ├─ ShoppingDb (PostgreSQL) - persistent data
    │   ├─ Orders
    │   ├─ OrderItems
    │   ├─ Cards/PaymentMethods
    │   └─ Transactions
    └─ Redis Cache - high-speed
        └─ Shopping Carts (volatile)

Donor Service
    └─ DonorDb (PostgreSQL)
        ├─ Donors/Contributors
        ├─ Donations
        ├─ Winners
        ├─ Drawings/Lottery
        └─ DonorStats

Analytics Service
    └─ AnalyticsDb (MongoDB/ClickHouse) - denormalized
        ├─ OrderMetrics
        ├─ UserBehavior
        ├─ SalesReports
        └─ DonorStatistics

Notification Service
    └─ NotificationDb (PostgreSQL) - optional logging
        ├─ NotificationLogs
        ├─ EmailTemplates
        └─ UserPreferences
```

### 4.2 Cross-Service Data Access

```
❌ BAD: Direct database access
├─ Shopping Service queries CatalogDb directly
└─ Tight coupling, hard to scale independently

✅ GOOD: API calls through service layer
├─ Shopping Service calls Catalog Service API
├─ Caches response in Redis
└─ Loose coupling, easier to maintain
```

---

## 5. Deployment & Infrastructure

### 5.1 Docker Containerization

```dockerfile
# Example: Shopping Service Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 5003

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["ShoppingService.csproj", "."]
RUN dotnet restore "ShoppingService.csproj"
COPY . .
RUN dotnet build "ShoppingService.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ShoppingService.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ShoppingService.dll"]
```

### 5.2 Kubernetes Orchestration

```yaml
# Example: Shopping Service Deployment
apiVersion: apps/v1
kind: Deployment
metadata:
  name: shopping-service
  namespace: default
spec:
  replicas: 3
  selector:
    matchLabels:
      app: shopping-service
  template:
    metadata:
      labels:
        app: shopping-service
    spec:
      containers:
      - name: shopping-service
        image: shopping-service:latest
        ports:
        - containerPort: 5003
        env:
        - name: DATABASE_URL
          valueFrom:
            secretKeyRef:
              name: db-secrets
              key: shopping-db-url
        - name: REDIS_URL
          valueFrom:
            configMapKeyRef:
              name: shopping-config
              key: redis-url
        - name: RABBITMQ_URL
          valueFrom:
            secretKeyRef:
              name: messaging-secrets
              key: rabbitmq-url
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
        livenessProbe:
          httpGet:
            path: /health
            port: 5003
          initialDelaySeconds: 30
          periodSeconds: 10
---
apiVersion: v1
kind: Service
metadata:
  name: shopping-service
spec:
  selector:
    app: shopping-service
  ports:
  - protocol: TCP
    port: 80
    targetPort: 5003
  type: ClusterIP
```

### 5.3 Service Discovery

```
Kubernetes DNS (Built-in)
├─ user-service.default.svc.cluster.local:80
├─ catalog-service.default.svc.cluster.local:80
├─ shopping-service.default.svc.cluster.local:80
├─ donor-service.default.svc.cluster.local:80
├─ notification-service.default.svc.cluster.local:80
└─ api-gateway.default.svc.cluster.local:80

Or: Consul (if not using K8s)
```

---

## 6. Cross-Cutting Concerns

### 6.1 Observability (Logging, Monitoring, Tracing)

```
Technology Stack:
├─ Logging: Serilog → ELK Stack (Elasticsearch, Logstash, Kibana)
│  └─ Structure: {timestamp, service, level, message, context}
│
├─ Monitoring: Prometheus + Grafana
│  └─ Metrics: Request count, latency, errors, throughput
│
├─ Tracing: Jaeger / Zipkin
│  └─ Track requests across all services
│
└─ Alerting: Alertmanager
   └─ Alert rules for SLA violations
```

### 6.2 Security

```
├─ Authentication: JWT Tokens
│  └─ Issued by User Service
│  └─ Validated by API Gateway
│
├─ Authorization: Role-Based Access Control (RBAC)
│  └─ Stored in User Service
│  └─ Checked by each service
│
├─ Service-to-Service: mTLS (mutual TLS)
│  └─ Encrypted communication between services
│
├─ Secrets Management: HashiCorp Vault / Azure Key Vault
│  └─ Database passwords, API keys, certificates
│
├─ API Gateway Security:
│  └─ API Key validation
│  └─ Rate limiting (100 req/min per user)
│  └─ CORS handling
│  └─ Input validation
│
└─ Network: Kubernetes Network Policies
   └─ Restrict traffic between services
```

### 6.3 Resilience Patterns

```
Applied to all inter-service calls:

├─ Circuit Breaker (Polly)
│  └─ Fail fast if service unavailable
│  └─ Prevent cascade failures
│
├─ Retry Logic
│  └─ Exponential backoff: 1s, 2s, 4s, 8s
│  └─ Max 3 retries
│
├─ Timeout
│  └─ All calls: 10 second timeout
│  └─ Fail if no response
│
├─ Bulkhead
│  └─ Limit concurrent requests to 50
│  └─ Prevent resource exhaustion
│
└─ Graceful Degradation
   └─ Return cached data if service down
   └─ Partial response better than error
```

---

## 7. Migration Roadmap

### Phase 1: Foundation (חודשים 1-2)
```
✓ Restructure codebase for microservices
✓ Set up API Gateway (Ocelot)
✓ Create shared libraries (DTOs, logging, Polly policies)
✓ Set up RabbitMQ infrastructure
✓ Implement service discovery pattern
```

### Phase 2: Extract Core Services (חודשים 3-4)
```
✓ Extract User Service (with JWT token generation)
✓ Extract Catalog Service (read-optimized with caching)
✓ Set up inter-service HTTP communication
✓ Implement authentication flow through API Gateway
```

### Phase 3: Extract Shopping Service (חודשים 5-6)
```
✓ Extract Shopping Service (basket + orders + payments)
✓ Integrate Redis for cart caching
✓ Implement Saga pattern for checkout
✓ Integrate with Payment Gateway (Stripe/PayPal)
✓ Add resilience patterns (Circuit Breaker, Retry)
```

### Phase 4: Extract Specialized Services (חודשים 7-8)
```
✓ Extract Donor Service (donors + winners + lottery)
✓ Set up message queue for async events
✓ Implement event publishing/subscribing
✓ Create domain events (OrderCreated, DonationRecorded, etc.)
```

### Phase 5: Supporting Services (חודשים 9-10)
```
✓ Implement Notification Service (email/SMS)
✓ Add Analytics Service (optional but recommended)
✓ Set up centralized logging (ELK Stack)
✓ Implement distributed tracing (Jaeger)
✓ Set up monitoring & alerting (Prometheus + Grafana)
```

### Phase 6: Containerization & Deployment (חודשים 11-12)
```
✓ Dockerize all services
✓ Set up Kubernetes cluster (or Docker Swarm)
✓ Implement CI/CD pipeline (GitHub Actions)
✓ Load testing & performance optimization
✓ Security hardening (mTLS, secrets management)
```

---

## 8. Tools & Technologies

### Backend Framework
```
✅ .NET 9 (ASP.NET Core)
✅ gRPC for high-performance internal communication (optional)
✅ Entity Framework Core for data access
```

### Data Management
```
PostgreSQL    → User, Catalog, Shopping, Donor services
Redis         → Caching (shopping cart, session)
MongoDB       → Analytics (denormalized data)
```

### Communication & Messaging
```
REST API      → Synchronous calls between services
RabbitMQ      → Asynchronous events (order, donation, etc.)
gRPC          → Optional: high-performance internal calls
```

### Infrastructure & DevOps
```
Docker        → Containerization
Kubernetes    → Container orchestration (or Docker Compose for dev)
Consul        → Service discovery (if not using K8s)
HashiCorp Vault → Secrets management
```

### Observability & Monitoring
```
Serilog       → Structured logging
Elasticsearch → Log storage & indexing
Logstash      → Log pipeline
Kibana        → Log visualization
Prometheus   → Metrics collection
Grafana       → Metrics visualization
Jaeger        → Distributed tracing
Alertmanager  → Alerting
```

### Resilience & Testing
```
Polly         → Circuit breaker, retry, timeout policies
xUnit         → Unit testing
Testcontainers → Integration testing with Docker
Postman/Rest Client → API testing
```

### Security
```
JWT           → Token-based authentication
OAuth2        → For third-party integrations
mTLS          → Service-to-service encryption
Azure Key Vault / HashiCorp Vault → Secrets management
OWASP Standards → Security guidelines
```

---

## 9. Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                           Clients                                   │
│                 (Web, Mobile, Third-party)                         │
└──────────────────────────────┬──────────────────────────────────────┘
                               │
                               ↓
                    ┌──────────────────────┐
                    │   API Gateway        │
                    │  (Ocelot/Kong)       │
                    │ - JWT Validation     │
                    │ - Rate Limiting      │
                    │ - Request Routing    │
                    └──────┬───────────────┘
                           │
         ┌─────────────────┼─────────────────┐
         │                 │                 │
         ↓                 ↓                 ↓
  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐
  │ User Service │  │Catalog Srvc  │  │Shopping Srvc │
  ├──────────────┤  ├──────────────┤  ├──────────────┤
  │ PostgreSQL   │  │ PostgreSQL   │  │ PostgreSQL   │
  │              │  │ + Redis      │  │ + Redis      │
  └──────────────┘  │ (Caching)    │  │ (Cart Cache) │
                    └──────────────┘  └──────────────┘
         │
         ↓
  ┌──────────────┐  ┌──────────────┐
  │Donor Service │  │Notification  │
  ├──────────────┤  │Service       │
  │ PostgreSQL   │  ├──────────────┤
  │              │  │RabbitMQ      │
  └──────────────┘  │(async only)  │
                    └──────────────┘
         │
         ↓
  ┌──────────────────┐
  │Analytics Service │
  ├──────────────────┤
  │MongoDB/ClickHouse│
  │(Event consumer)  │
  └──────────────────┘

┌────────────────────────────────────────────────────────────────┐
│                   Message Queue (RabbitMQ)                     │
│  - OrderCreated, OrderConfirmed, PaymentProcessed             │
│  - DonationRecorded, WinnerDrawn, NotificationSent            │
└────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────┐
│              Observability Stack (Shared)                       │
│  - ELK Stack (Logging)                                         │
│  - Prometheus + Grafana (Monitoring)                           │
│  - Jaeger (Distributed Tracing)                               │
└────────────────────────────────────────────────────────────────┘
```

---

## 10. Challenges & Solutions

| Challenge | Solution |
|-----------|----------|
| **Distributed Transactions** | Saga pattern with RabbitMQ, event-driven |
| **Data Consistency** | Eventual consistency model, event sourcing |
| **Network Latency** | Caching (Redis), async processing, batch operations |
| **Service Failures** | Circuit breaker pattern, retry logic, graceful degradation |
| **Operational Complexity** | Docker + Kubernetes, centralized monitoring |
| **Testing** | Contract testing, test doubles, test containers |
| **Debugging** | Distributed tracing (Jaeger), centralized logging |
| **Secrets Management** | Vault, never hardcode credentials |
| **Data Migration** | Strangler Fig pattern - migrate data gradually |

---

## 11. Success Metrics

- ⏱️ **API Response Time**: < 200ms for 95th percentile
- 📊 **Service Availability**: 99.9% uptime (Three 9s) per service
- 📈 **Scalability**: Handle 10x current load independently per service
- 🔄 **Deployment Frequency**: Multiple times per week per service
- 🐛 **MTTR** (Mean Time To Repair): < 30 minutes for critical issues
- 🔍 **Error Rate**: < 0.1% of requests
- 💾 **Database Query Time**: < 100ms for 95th percentile

---

## 12. Team Structure (Recommended)

```
Frontend Team
    ↓ (API calls via Gateway)

API Gateway Team
    ↓
┌─────────────────────────────────────────────────────────┐
│                                                         │
├─ User Service Team        (1-2 engineers)              │
├─ Catalog Service Team     (1 engineer)                 │
├─ Shopping Service Team    (2-3 engineers) - most busy  │
├─ Donor Service Team       (1-2 engineers)              │
├─ Notification Service Team (1 engineer)                │
└─ DevOps/Platform Team     (1-2 engineers)              │
    ├─ Kubernetes, CI/CD, Monitoring
    └─ Shared Infrastructure, RabbitMQ, Redis
```

---

## 13. References & Resources

- [Microservices Patterns by Chris Richardson](https://microservices.io/)
- [Domain-Driven Design - Eric Evans](https://en.wikipedia.org/wiki/Domain-driven_design)
- [Saga Pattern](https://microservices.io/patterns/data/saga.html)
- [API Gateway Pattern](https://microservices.io/patterns/apigateway.html)
- [.NET Microservices e-book](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/)
- [Kubernetes Documentation](https://kubernetes.io/docs/)
- [RabbitMQ - Message Broker](https://www.rabbitmq.com/)
- [Observability with Distributed Tracing](https://www.jaegertracing.io/)

---

**סטטוס:** 📝 תכנון בלבד - טרם מומש

**הערה:** מסמך זה הוא מבט עתידי על אך ורק. היה אחד הבשלות את התכנון כאשר המערכת תגדל ותגיע ל-production.

Last Updated: June 2026
