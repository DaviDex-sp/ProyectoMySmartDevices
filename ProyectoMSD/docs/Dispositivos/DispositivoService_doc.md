# DispositivoService — Documentation

## Purpose

`DispositivoService` is the single source of truth for all device-related data access in the MySmartDevice project. It implements `IDispositivoService` and acts as the exclusive intermediary between the database (`AppDbContext`) and any PageModel or Controller that needs device data. No PageModel or Controller may access `AppDbContext` directly for device operations.

## Dependencies

| Dependency | Type | Notes |
|---|---|---|
| `AppDbContext` | EF Core DbContext | Injected via DI (`Scoped`) |
| `Microsoft.EntityFrameworkCore` | NuGet | LINQ query support |
| `ProyectoMSD.Modelos.DTOs.DispositivoDto` | Internal DTO | Data transfer contract |
| `ProyectoMSD.Modelos.Dispositivo` | Entity | EF Core entity model |

## Public API / Interface

### `GetDispositivosAsync()`
```csharp
Task<List<DispositivoDto>> GetDispositivosAsync()
```
Returns all devices as a list of `DispositivoDto`. Uses `AsNoTracking()` for read-only performance. Populates all fields including the IoT fields (`MAC_Address`, `Protocolo`, `IdEspacio`) and UI helpers (`IconClass`, `BadgeClass`, `IsActive`).

---

### `GetTotalDispositivosAsync()`
```csharp
Task<int> GetTotalDispositivosAsync()
```
Returns the total device count. Used by the Dashboard analytics layer.

---

### `ToggleEstadoAsync(int id)`
```csharp
Task<bool> ToggleEstadoAsync(int id)
```
Toggles the device state between `"Activo"` and `"Inactivo"`. Returns `true` on success, `false` if the device was not found or a `DbUpdateException` occurred.

| Parameter | Type | Description |
|---|---|---|
| `id` | `int` | Primary key of the device to toggle |

---

### `GetByIdAsync(int id)` *(New)*
```csharp
Task<DispositivoDto?> GetByIdAsync(int id)
```
Fetches a single device by its primary key using `AsNoTracking()`. Returns a fully-mapped `DispositivoDto` or `null` if no device is found. Used by the Edit page to pre-populate the form.

| Parameter | Type | Description |
|---|---|---|
| `id` | `int` | Primary key of the device to fetch |

**Returns:** `DispositivoDto?` — all fields populated including `IdEspacio`, `MAC_Address`, `Protocolo`.

---

### `UpdateAsync(DispositivoDto dto)` *(New)*
```csharp
Task<bool> UpdateAsync(DispositivoDto dto)
```
Finds the tracked entity by `dto.Id`, applies all field assignments explicitly, and persists via `SaveChangesAsync()`. Returns `true` on success. Returns `false` if the device was not found or a `DbUpdateException` occurred. The PageModel is responsible for displaying an error message when `false` is returned.

| Parameter | Type | Description |
|---|---|---|
| `dto` | `DispositivoDto` | DTO carrying all updated field values |

**Fields updated:** `IdEspacio`, `MAC_Address`, `Protocolo`, `Nombre`, `Tipo`, `Marca`, `Usos`, `Estado`.

---

## Usage Example

### Edit PageModel (thin orchestrator pattern)
```csharp
// OnGetAsync
var dispositivo = await _dispositivoService.GetByIdAsync(id.Value);
if (dispositivo == null) return NotFound();
Dispositivo = dispositivo;

// OnPostAsync
var success = await _dispositivoService.UpdateAsync(Dispositivo);
if (!success)
{
    ModelState.AddModelError(string.Empty, "No se pudo actualizar el dispositivo.");
    return Page();
}
return RedirectToPage("./Index");
```

## Architectural Notes

- `GetByIdAsync` and `GetDispositivosAsync` use `AsNoTracking()` — they are read-only queries. EF Core will not track these instances.
- `UpdateAsync` uses `FindAsync(dto.Id)` which returns a tracked entity, enabling change detection on `SaveChangesAsync()`.
- The service is registered as `Scoped` — one instance per HTTP request, sharing the same `AppDbContext` within that request lifetime.
