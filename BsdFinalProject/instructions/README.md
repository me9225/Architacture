# 📋 Copilot Instructions & Documentation

This directory contains structured guidance for GitHub Copilot and development team regarding the BsdFinalProject architecture and best practices.

## 📁 Files Overview

### 📖 Guidance Documents (Reference Material)

#### 1. **instructions.md** - Main Project Guidelines
- Project overview (Donation/Gift management system on .NET 9)
- General coding standards and conventions
- Database and API design guidelines
- Token conservation tips
- **Use this:** For general project questions

#### 2. **repositories.md** - Repository Layer Deep Dive
- Repository Pattern implementation details
- List of all 8 repositories (Basket, Card, Category, Donor, Gift, Manager, User, Winner)
- Standard repository structure and patterns
- Async/await requirements (critical!)
- Null handling and SaveChangesAsync() patterns
- Common errors to avoid
- Performance considerations
- **Use this:** When working with data access layer

#### 3. **controllers.md** - Controller Layer Deep Dive
- Controller structure and patterns
- HTTP methods with proper status codes
- Authentication & user extraction
- DTO usage (never return Models directly)
- Complete CRUD examples
- Error handling strategies
- Best practices and common pitfalls
- **Use this:** When working with API endpoints

#### 4. **microservices-architecture.md** - Future Architecture Planning
- **Status:** Planning phase only - NOT YET IMPLEMENTED
- Current monolithic structure vs. proposed 6-service model
- Service boundaries and responsibilities
- Communication patterns (Sync REST + Async RabbitMQ)
- Database-per-service strategy
- Saga Pattern for transactions
- Docker & Kubernetes deployment
- 12-month migration roadmap
- **Use this:** For understanding long-term scalability

---

### 🤖 Custom Copilot Agents (Interactive)

#### 5. **microservices-architect.agent.md** - Microservices Design Agent
**Activate with:** "Microservices Architect mode"

**Helps with:**
- Planning new microservice extraction
- Designing service boundaries
- Implementing inter-service communication
- Event-driven architecture patterns
- Distributed transactions (Saga)
- Resilience patterns (Circuit Breaker, Retry)

**Key Features:**
- Information gathering before implementation
- DDD (Domain-Driven Design) alignment
- Three-tier architecture (Repo, Service, Controller)
- Communication pattern guidance
- Quality checklist

**Example Usage:**
```
Me: "Microservices Architect mode - I want to build the Notification Service"
Agent: "Great! I need the following information... [questions]"
Me: "[Provide details]"
Agent: "Got it! A few clarifying questions..."
Me: "Generate Notification Service"
Agent: [Produces complete architecture + code]
```

---

#### 6. **data-architect.agent.md** - Data Design Agent
**Activate with:** "Data Architect mode"

**Helps with:**
- Database schema design per microservice
- Choosing right database (PostgreSQL, Redis, MongoDB)
- Entity relationships and aggregates
- Entity Framework migrations
- Query optimization
- Denormalization for analytics
- Data consistency strategies
- Event sourcing patterns

**Key Features:**
- Database selection framework (transactional vs. analytical)
- Entity design patterns (Aggregate Root, Value Objects)
- Full schema examples for each service
- Redis cache key patterns
- Query optimization guidelines
- EF Core best practices

**Example Usage:**
```
Me: "Data Architect mode - Design shopping service database"
Agent: "I need some information... [questions]"
Me: "[Provide access patterns, volume, consistency needs]"
Agent: "Perfect! Based on your requirements, here's the design..."
[Shows schema, entities, migrations]
```

---

#### 7. **security-architect.agent.md** - Security Design Agent
**Activate with:** "Security Architect mode"

**Helps with:**
- Authentication system design (JWT, OAuth2, API Keys)
- Authorization (Role-Based RBAC, Attribute-Based ABAC)
- Secrets management (Key Vault, Vault, environment variables)
- OWASP Top 10 prevention
- Service-to-service security (mTLS)
- Rate limiting & DDoS protection
- Data encryption (at-rest and in-transit)
- Audit logging for compliance

**Key Features:**
- Complete JWT implementation examples
- OAuth2/OpenID Connect setup
- All 10 OWASP vulnerabilities with solutions
- Secrets management best practices
- Security checklist for deployment

**Example Usage:**
```
Me: "Security Architect mode - Secure the API"
Agent: "Let me understand your security requirements... [questions]"
Me: "[Answer about data sensitivity, compliance]"
Agent: "Based on your needs, here's the security architecture..."
[Shows JWT setup, authorization, OWASP prevention]
```

---

## 🎯 Quick Navigation

### By Role/Task

| Task | Document | Agent |
|------|----------|-------|
| **General coding** | `instructions.md` | - |
| **Data access layer** | `repositories.md` | - |
| **API endpoints** | `controllers.md` | - |
| **Long-term planning** | `microservices-architecture.md` | - |
| **Building a service** | - | `microservices-architect.agent` |
| **Database design** | - | `data-architect.agent` |
| **Security implementation** | - | `security-architect.agent` |

### By Problem

| Problem | Solution |
|---------|----------|
| "Repository code is slow" | Read `repositories.md` → Use indexes |
| "API returns 500 errors" | Read `controllers.md` → Error handling section |
| "I need to build a new service" | Say "Microservices Architect mode" |
| "Database design questions" | Say "Data Architect mode" |
| "Security concerns" | Say "Security Architect mode" |
| "Understanding microservices" | Read `microservices-architecture.md` |

---

## 🤖 How to Use Agents

### Step 1: Activate Agent
```
"Microservices Architect mode"
or
"Data Architect mode"
or
"Security Architect mode"
```

### Step 2: Provide Context
Agent will ask clarifying questions. Answer them thoroughly with:
- Requirements and constraints
- Current architecture/situation
- Goals and success criteria
- Any specific technologies or preferences

### Step 3: Request Generation
Once information is gathered:
- "Generate [ServiceName]"
- "Design schema for [Entity]"
- "Implement authentication"
- Etc.

### Step 4: Get Implementation
Agent produces:
- Architecture Decision Records (ADR)
- Service contracts and DTOs
- Full working code (no templates!)
- Configuration guidance
- Testing strategy

---

## 📚 Reference Architecture

### Current: Monolithic
```
BsdFinalProject (Single Service)
├── Controllers/ (9 endpoints)
├── Repositories/ (8 data access)
├── Services/ (business logic)
└── Database/ (single SQL Server)
```

### Future: 6-Service Microservices
| Service | Purpose | DB | Status |
|---------|---------|----|----|
| 🧑‍💼 User Service | Auth & users | PostgreSQL | 📝 Plan |
| 📚 Catalog Service | Products & categories | PostgreSQL + Redis | 📝 Plan |
| 🛍️ Shopping Service | Cart, orders, payments | PostgreSQL + Redis | 📝 Plan |
| 🤝 Donor Service | Donors, winners, lottery | PostgreSQL | 📝 Plan |
| 📬 Notification Service | Email, SMS, alerts | PostgreSQL | 📝 Plan |
| 📊 Analytics Service | Reports & metrics | MongoDB | 📝 Plan |
| 🚪 API Gateway | Routing & auth | - | 📝 Plan |

---

## 🚀 Workflow Examples

### Example 1: Build a New Microservice

```
1. Activate: "Microservices Architect mode"
   ↓
2. Agent asks: Service name, responsibility, entities, endpoints, etc.
   ↓
3. You provide: All required information
   ↓
4. Say: "Generate [ServiceName]"
   ↓
5. Get: ADR, contracts, architecture decision
   ↓
6. Say: "Implement"
   ↓
7. Get: Full working code for all 3 layers
```

### Example 2: Design Service Database

```
1. Activate: "Data Architect mode"
   ↓
2. Agent asks: Access patterns, data volume, consistency needs
   ↓
3. You provide: Requirements
   ↓
4. Say: "Design schema for [ServiceName]"
   ↓
5. Get: Complete SQL schema with indexes
   ↓
6. Say: "Create migration"
   ↓
7. Get: EF Core migration files ready to run
```

### Example 3: Secure an Endpoint

```
1. Activate: "Security Architect mode"
   ↓
2. Agent asks: Data sensitivity, compliance, authentication method
   ↓
3. You provide: Security requirements
   ↓
4. Say: "Implement JWT authentication"
   ↓
5. Get: Full JWT setup with token generation and validation
   ↓
6. Say: "Add role-based authorization"
   ↓
7. Get: RBAC implementation with policy-based attributes
```

---

## ✅ Quality Standards

All code produced follows these standards:
- ✅ .NET 9 / ASP.NET Core
- ✅ Async/await throughout (no sync blocking)
- ✅ DTOs for all requests/responses
- ✅ Proper HTTP status codes
- ✅ Repository Pattern for data access
- ✅ Dependency Injection
- ✅ Structured logging
- ✅ Input validation
- ✅ Error handling
- ✅ No hardcoded secrets
- ✅ Security-first design
- ✅ OWASP Top 10 compliant

---

## 🔗 External Resources

- [GitHub Awesome Copilot](https://github.com/github/awesome-copilot)
- [Microservices Patterns](https://microservices.io/)
- [Domain-Driven Design](https://en.wikipedia.org/wiki/Domain-driven_design)
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [Entity Framework Core Docs](https://docs.microsoft.com/en-us/ef/core/)

---

## 📞 Support

**For questions about:**
- **General standards:** → Read `instructions.md`
- **Coding patterns:** → Read `repositories.md` or `controllers.md`
- **Architecture decisions:** → Read `microservices-architecture.md`
- **Building services:** → Use "Microservices Architect mode"
- **Data design:** → Use "Data Architect mode"
- **Security setup:** → Use "Security Architect mode"

---

## 📊 Files Summary

```
📂 instructions/
├── README.md                              ← You are here
├── 📖 instructions.md                     (General guidelines)
├── 📖 repositories.md                     (Repository patterns)
├── 📖 controllers.md                      (API endpoint patterns)
├── 📖 microservices-architecture.md       (Future architecture)
├── 🤖 microservices-architect.agent.md   (Service design agent)
├── 🤖 data-architect.agent.md            (Database design agent)
└── 🤖 security-architect.agent.agent.md  (Security design agent)

Legend:
📖 = Reference documentation (read for learning/reference)
🤖 = Interactive agents (activate and converse with Copilot)
```

---

## 🎓 Learning Path

1. **Start here:** Read `instructions.md` for project context
2. **Learn code patterns:** Read `repositories.md` + `controllers.md`
3. **Understand future:** Read `microservices-architecture.md`
4. **Build something:** Use "Microservices Architect mode"
5. **Design database:** Use "Data Architect mode"
6. **Secure it:** Use "Security Architect mode"

---

**Repository:** https://github.com/me9225/DotnetAngularProject  
**Branch:** master  
**Last Updated:** June 2026

---

© 2026 BsdFinalProject Team | Comprehensive Architecture & Development Guide
