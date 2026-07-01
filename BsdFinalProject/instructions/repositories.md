# Copilot Instructions - Repositories

This document provides specific guidance for working with the Repository layer in BsdFinalProject.

## Repository Pattern Implementation

The project uses the Repository Pattern to abstract data access from business logic.

## Repository Files

Located in `BsdFinalProject/Repositories/`:

- **BasketRepository.cs** - Manages shopping basket data (user baskets, additions, removals)
- **CardRepository.cs** - Handles card/payment information
- **CategoryRepository.cs** - Manages gift categories
- **DonorRepository.cs** - Manages donor profiles and information
- **GiftRepository.cs** - Handles gift management and queries
- **ManagerRepository.cs** - Manages administrative users
- **UserRepository.cs** - Handles user account operations
- **WinnerRepository.cs** - Manages gift recipient information

## Repository Interfaces

Located in `BsdFinalProject/IRepository/`:

Each repository has a corresponding interface (e.g., `IBasketRepository`, `IGiftRepository`, etc.) that defines the contract.

## Standard Repository Structure

```csharp
public class [Entity]Repository : I[Entity]Repository
{
    private SaleContext _context = SaleContextFactory.CreateContext();

    // CRUD operations
    public async Task<[Entity]> CreateAsync([Entity] entity)
    public async Task<[Entity]> GetByIdAsync(int id)
    public async Task<IEnumerable<[Entity]>> GetAllAsync()
    public async Task<[Entity]> UpdateAsync([Entity] entity)
    public async Task<bool> DeleteAsync(int id)

    // Specialized queries
    public async Task<IEnumerable<[Entity]>> GetByConditionAsync(Expression<Func<[Entity], bool>> predicate)
}
```

## Key Patterns in Current Repositories

### 1. Context Initialization
```csharp
SaleContext _context = SaleContextFactory.CreateContext();
```
- All repositories use `SaleContextFactory.CreateContext()` to initialize the database context
- Do NOT create `new SaleContext()` directly

### 2. Async Operations
```csharp
public async Task<IEnumerable<Basket>> GetAllMyBasket(int Id)
{
    return await _context.Basket
        .Where(b => b.UserId == Id)
        .ToListAsync();
}
```
- All database operations must be `async`
- Always call `.ToListAsync()`, `.FirstOrDefaultAsync()`, `.FindAsync()`, etc.
- Return `Task<T>` or `Task<IEnumerable<T>>`

### 3. Saving Changes
```csharp
await _context.SaveChangesAsync();
```
- Always use `SaveChangesAsync()` - never use synchronous `SaveChanges()`
- Call after every Add, Remove, or Update operation

### 4. Null Handling
```csharp
var basket = await _context.Basket.FindAsync(id);
if (basket == null) return null;
```
- Always check for null before proceeding
- Return null or appropriate default value if entity not found

### 5. Batch Operations
```csharp
_context.Basket.RemoveRange(baskets);
await _context.SaveChangesAsync();
```
- Use `AddRange()` and `RemoveRange()` for bulk operations
- More efficient than individual operations

## Common Repository Methods

### Read Operations
- `GetAllAsync()` - Retrieve all entities
- `GetByIdAsync(int id)` - Get single entity by ID
- `FindAsync()` - Direct context method for ID lookup
- `Where()` - Filter with LINQ predicate

### Write Operations
- `Add(entity)` - Insert new entity
- `AddRange(entities)` - Insert multiple
- `Remove(entity)` - Delete single entity
- `RemoveRange(entities)` - Delete multiple
- `Update(entity)` - Modify existing entity

### Important
- Always call `SaveChangesAsync()` after modifications
- Don't forget `await` on async calls

## Database Context

**DbSet Properties in SaleContext:**
- `DbSet<Basket>` - _context.Basket
- `DbSet<Card>` - _context.Card
- `DbSet<Category>` - _context.Category
- `DbSet<Donor>` - _context.Donor
- `DbSet<Gift>` - _context.Gift
- `DbSet<Manager>` - _context.Manager
- `DbSet<User>` - _context.User
- `DbSet<Winner>` - _context.Winner

## Error Scenarios

### Null Reference
```csharp
// BAD - no null check
var user = await _context.User.FindAsync(userId);
_context.User.Remove(user); // Could fail if user is null

// GOOD
var user = await _context.User.FindAsync(userId);
if (user == null) return false;
_context.User.Remove(user);
await _context.SaveChangesAsync();
```

### Missing Await
```csharp
// BAD - missing await
public async Task<User> GetUser(int id)
{
    return _context.User.FindAsync(id); // Returns Task<User>, not User
}

// GOOD
public async Task<User> GetUser(int id)
{
    return await _context.User.FindAsync(id);
}
```

### Forgetting SaveChangesAsync
```csharp
// BAD - no save called
_context.Basket.Add(basket);
return basket; // Changes not persisted

// GOOD
_context.Basket.Add(basket);
await _context.SaveChangesAsync();
return basket;
```

## Adding New Repositories

When creating a new repository for a new entity:

1. Create the entity model in `Models/[Entity].cs`
2. Add `DbSet<[Entity]>` to `SaleContext`
3. Create interface `I[Entity]Repository` in `IRepository/` folder
4. Create `[Entity]Repository.cs` following the pattern above
5. Register in dependency injection (Startup/Program.cs)
6. Use in Services/Controllers via DI

## Performance Considerations

### Efficient Queries
```csharp
// BAD - loads all baskets then filters in memory
var allBaskets = await _context.Basket.ToListAsync();
var userBaskets = allBaskets.Where(b => b.UserId == id);

// GOOD - filters at database level
var userBaskets = await _context.Basket
    .Where(b => b.UserId == id)
    .ToListAsync();
```

### Include Related Data When Needed
```csharp
// If you need related entities, use Include
var gifts = await _context.Gift
    .Include(g => g.Category)
    .ToListAsync();
```

## Testing Queries

When adding new query methods:
1. Test with edge cases (empty results, null values)
2. Verify SQL generated by EF Core
3. Check for N+1 query problems
4. Validate async/await usage

---

**For questions about implementing specific repository methods, refer to the existing implementations in the Repositories folder as examples.**

Last Updated: June 2026
