# Copilot Instructions - Controllers

This document provides specific guidance for working with the Controller layer in BsdFinalProject.

## Controllers Overview

Located in `BsdFinalProject/Controllers/`, controllers handle HTTP requests and orchestrate the API responses.

## Controller Files

- **BasketsController.cs** - Shopping basket endpoints
- **CardsController.cs** - Card/payment endpoints
- **CategoriesController.cs** - Category management endpoints
- **DonorsController.cs** - Donor management endpoints
- **GiftsController.cs** - Gift management endpoints
- **ManegersController.cs** - Manager/admin endpoints
- **UsersController.cs** - User account endpoints
- **WinnersController.cs** - Winner management endpoints
- **WeatherForecastController.cs** - Sample endpoint (can be removed)

## Standard Controller Structure

```csharp
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace FinalProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // If all endpoints require auth
    public class [Entity]Controller : ControllerBase
    {
        private readonly [Entity]Service _service;
        private readonly SaleContext _context;

        public [Entity]Controller([Entity]Service service, SaleContext context)
        {
            _service = service;
            _context = context;
        }

        // HTTP endpoints here
    }
}
```

## Authentication & Authorization

### Getting Current User ID
```csharp
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (userId == null)
    return Unauthorized();

int userId = int.Parse(userId);
```
- Use `ClaimTypes.NameIdentifier` to get the authenticated user's ID
- Check for null before using
- Parse to `int` when needed

### Protect Individual Endpoints
```csharp
[Authorize] // All users
[Authorize(Roles = "Admin")] // Specific roles
public async Task<ActionResult<IEnumerable<ManagerDto>>> Get() { }
```

## HTTP Methods & Status Codes

### GET Endpoints
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<BasketDto>>> GetAll()
{
    var result = await _service.GetAllAsync();
    return Ok(result); // 200 OK
}

[HttpGet("{id}")]
public async Task<ActionResult<BasketDto>> GetById(int id)
{
    var result = await _service.GetByIdAsync(id);
    if (result == null)
        return NotFound(); // 404
    return Ok(result);
}
```
- Use `Ok()` for successful retrieval (200)
- Use `NotFound()` when entity doesn't exist (404)

### POST Endpoints
```csharp
[HttpPost]
public async Task<ActionResult<BasketDto>> Create([FromBody] CreateBasketDto dto)
{
    if (!ModelState.IsValid)
        return BadRequest(ModelState); // 400

    var result = await _service.CreateAsync(dto);
    return CreatedAtAction(nameof(GetById), new { id = result.Id }, result); // 201
}
```
- Use `BadRequest()` for validation errors (400)
- Use `CreatedAtAction()` for successful creation (201)
- Include `[FromBody]` for request body DTOs

### PUT Endpoints
```csharp
[HttpPut("{id}")]
public async Task<IActionResult> Update(int id, [FromBody] UpdateBasketDto dto)
{
    var result = await _service.UpdateAsync(id, dto);
    if (result == null)
        return NotFound();
    return NoContent(); // 204
}
```
- Use `NoContent()` for successful update with no return body (204)
- Or return the updated entity with `Ok(result)`

### DELETE Endpoints
```csharp
[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    var success = await _service.DeleteAsync(id);
    if (!success)
        return NotFound();
    return NoContent(); // 204
}
```
- Use `NoContent()` for successful deletion (204)

## DTO Usage

**Never return Models directly - use DTOs:**

```csharp
// BAD
[HttpGet]
public async Task<ActionResult<IEnumerable<Basket>>> GetAll()
{
    return Ok(await _service.GetAllAsync()); // Returns Models
}

// GOOD
[HttpGet]
public async Task<ActionResult<IEnumerable<BasketDto>>> GetAll()
{
    return Ok(await _service.GetAllAsync()); // Returns DTOs
}
```

### Common DTO Naming Pattern
- `[Entity]Dto` - Full DTO with all properties
- `Create[Entity]Dto` - Input DTO for POST
- `Update[Entity]Dto` - Input DTO for PUT
- `[Entity]ResponseDto` - Response-specific DTO

## Example Controller Patterns

### Pattern 1: Standard CRUD
```csharp
[HttpGet]
public async Task<ActionResult<IEnumerable<BasketDto>>> GetAll()
{
    var baskets = await _service.GetAllAsync();
    return Ok(baskets);
}

[HttpGet("{id}")]
public async Task<ActionResult<BasketDto>> GetById(int id)
{
    var basket = await _service.GetByIdAsync(id);
    if (basket == null)
        return NotFound();
    return Ok(basket);
}

[HttpPost]
public async Task<ActionResult<BasketDto>> Create([FromBody] CreateBasketDto dto)
{
    var basket = await _service.CreateAsync(dto);
    return CreatedAtAction(nameof(GetById), new { id = basket.Id }, basket);
}

[HttpPut("{id}")]
public async Task<IActionResult> Update(int id, [FromBody] UpdateBasketDto dto)
{
    var result = await _service.UpdateAsync(id, dto);
    if (result == null)
        return NotFound();
    return NoContent();
}

[HttpDelete("{id}")]
public async Task<IActionResult> Delete(int id)
{
    var success = await _service.DeleteAsync(id);
    if (!success)
        return NotFound();
    return NoContent();
}
```

### Pattern 2: User-Specific Operations
```csharp
[HttpGet("my-baskets")]
public async Task<ActionResult<IEnumerable<BasketDto>>> GetMyBaskets()
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userId == null)
        return Unauthorized();

    var baskets = await _service.GetUserBasketsAsync(int.Parse(userId));
    return Ok(baskets);
}
```

### Pattern 3: Action on Resource
```csharp
[HttpPost("{id}/checkout")]
public async Task<ActionResult<OrderDto>> Checkout(int id)
{
    var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    if (userId == null)
        return Unauthorized();

    var result = await _service.CheckoutAsync(id, int.Parse(userId));
    if (result == null)
        return BadRequest("Checkout failed");

    return Ok(result);
}
```

## Error Handling in Controllers

### Validation
```csharp
if (!ModelState.IsValid)
    return BadRequest(ModelState);
```

### Null Checks
```csharp
if (result == null)
    return NotFound();
```

### Authorization Checks
```csharp
var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
if (userId == null)
    return Unauthorized();
```

### Business Logic Errors
```csharp
try
{
    var result = await _service.ProcessAsync(data);
    if (!result.Success)
        return BadRequest(result.Message);
    return Ok(result.Data);
}
catch (Exception ex)
{
    return StatusCode(500, "Internal server error");
}
```

## Routing & Attributes

### Controller Route
```csharp
[Route("api/[controller]")] // api/baskets, api/gifts, etc.
[ApiController] // Always use for REST APIs
```

### Method Routing
```csharp
[HttpGet] // GET /api/baskets
[HttpGet("{id}")] // GET /api/baskets/{id}
[HttpGet("my-items")] // GET /api/baskets/my-items
[HttpPost] // POST /api/baskets
[HttpPut("{id}")] // PUT /api/baskets/{id}
[HttpDelete("{id}")] // DELETE /api/baskets/{id}
```

### Authorization
```csharp
[Authorize] // Requires authentication
[AllowAnonymous] // No authentication required
[Authorize(Roles = "Admin")] // Role-based
```

## Best Practices

### 1. Keep Controllers Thin
- Controllers should coordinate, not implement business logic
- Delegate to Services for processing
- Focus on HTTP concerns (routing, status codes, DTOs)

### 2. Consistent Naming
```csharp
// Action naming
public async Task<ActionResult<ItemDto>> GetAll()
public async Task<ActionResult<ItemDto>> GetById(int id)
public async Task<ActionResult<ItemDto>> Create([FromBody] CreateItemDto dto)
public async Task<IActionResult> Update(int id, [FromBody] UpdateItemDto dto)
public async Task<IActionResult> Delete(int id)
```

### 3. Always Use Async
```csharp
// BAD
public ActionResult<IEnumerable<BasketDto>> GetAll()
{
    return Ok(_service.GetAll());
}

// GOOD
public async Task<ActionResult<IEnumerable<BasketDto>>> GetAll()
{
    return Ok(await _service.GetAllAsync());
}
```

### 4. Proper Response Structure
```csharp
// Successful GET
return Ok(new { data = baskets }); // 200

// Successful POST
return CreatedAtAction(nameof(GetById), new { id = result.Id }, result); // 201

// Successful PUT/DELETE
return NoContent(); // 204

// Not found
return NotFound(); // 404

// Bad request
return BadRequest(new { error = "reason" }); // 400

// Unauthorized
return Unauthorized(); // 401
```

## Adding New Controller

1. Create `[Entity]Controller.cs` in Controllers/
2. Inherit from `ControllerBase`
3. Add `[ApiController]` and `[Route("api/[controller]")]` attributes
4. Inject `[Entity]Service` and `SaleContext` in constructor
5. Implement standard CRUD endpoints
6. Use DTOs for all request/response bodies
7. Return appropriate status codes

## Common Issues

### Issue: Forgetting Await
```csharp
// WRONG
var result = _service.GetAsync(id); // Returns Task<Entity>, not Entity
return Ok(result); // Serializes Task object

// RIGHT
var result = await _service.GetAsync(id);
return Ok(result);
```

### Issue: Returning Models Instead of DTOs
```csharp
// WRONG
public async Task<ActionResult<Basket>> GetAll()
{
    return Ok(await _context.Basket.ToListAsync());
}

// RIGHT
public async Task<ActionResult<IEnumerable<BasketDto>>> GetAll()
{
    return Ok(await _service.GetAllAsync());
}
```

### Issue: Missing Null Check
```csharp
// WRONG
var basket = await _service.GetByIdAsync(id);
return Ok(basket); // Could be null

// RIGHT
var basket = await _service.GetByIdAsync(id);
if (basket == null)
    return NotFound();
return Ok(basket);
```

---

**For specific controller implementation questions, refer to existing controllers as examples.**

Last Updated: June 2026
