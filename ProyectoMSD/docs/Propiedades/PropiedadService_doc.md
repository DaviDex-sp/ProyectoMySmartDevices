# PropiedadService y DTO (Refactorización SoC)

## Propósito
El servicio de `PropiedadService` centraliza las lógicas de negocio, recuperación de datos e integraciones complejas (como incluir entidades de espacios y usuarios vinculados) requeridas por los controladores o Razor Pages del módulo de Propiedades.

Como parte del mantenimiento estratégico (Anti-Patrón "Smart UI"), se ha depurado cualquier inyección de variables CSS (`BadgeClass`) en el `PropiedadDto`, forzando a que las decisiones de interfaz visual se manejen explícitamente en el lado del cliente (Vistas de Razor).

## Dependencias
- `AppDbContext`: Contexto central de bases de datos de Entity Framework.
- `IPropiedadService`: Interfaz o contrato de exposición.
- Entidades relacionadas: `Propiedade`, `UsuariosPropiedades`, `Espacio`, `Dispositivos`.

## API Specs (Firma de Métodos Principales)

### `GetPropiedadesAsync(bool isAdmin, int? userId)`
- **Uso:** Obtiene todas las propiedades o las filtradas para el usuario autenticado.
- **Retorno:** `Task<List<PropiedadDto>>`
- **Nota técnica de refactor:** Retorna la propiedad plana `Tipo` en el DTO sin mapearlo a clases CSS. La capa de presentación es ahora responsable de su tematización. 

### `GetTotalPropiedadesAsync()`
- **Uso:** Método estático de agregación para despliegues como el Dashboard.
- **Retorno:** `Task<int>`.

## Ejemplo de uso (Presentación)
```csharp
// En el backend (Controller/PageModel)
var props = await _propiedadService.GetPropiedadesAsync(isAdmin: false, userId: 1);

// En la UI (Razor Engine)
@{ 
   var t = item.Tipo?.ToLower(); 
   string bc = t == "casa" ? "bg-success" : "bg-primary";
}
<span class="badge @bc">@item.Tipo</span>
```

**Estado:** Refactorizado [Clean Architecture / SoC]
