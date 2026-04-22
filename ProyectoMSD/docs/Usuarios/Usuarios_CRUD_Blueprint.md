# Blueprint Arquitectónico: Módulo de Usuarios (CRUD y Gestión)

## A. Diagnóstico y Objetivo

**Diagnóstico Actual:**
Se ha auditado la implementación actual del módulo de Usuarios (`Pages/Usuarios`). Aunque se utiliza un servicio intermedio (`IUsuarioService`) para desacoplar el acceso a datos (`AppDbContext`) de los controladores (Razor Pages), se ha detectado el uso del anti-patrón de **Over-Posting**. Los métodos del CRUD actuales en `Create.cshtml.cs` y `Edit.cshtml.cs` utilizan `[BindProperty] public Usuario Usuario { get; set; }`, vinculando directamente la entidad del dominio a la vista. Adicionalmente, el servicio `IUsuarioService` espera directamente el modelo de dominio `Usuario` para operaciones de creación y actualización.

**Objetivo:**
Estandarizar y refactorizar el CRUD de Usuarios bajo las normativas de "Clean Architecture / N-Tier" estipuladas en `arch-1.md`. Se debe erradicar la exposición directa del modelo de dominio `Usuario` en las capas de presentación, introduciendo explícitamente DTOs (Data Transfer Objects) para las operaciones de ingreso (Input) y salida (Output). 

---

## B. Estructura de Directorios (Folder Structure)

Las refactorizaciones y nuevos archivos deben ubicarse en las siguientes rutas para respetar la separación de responsabilidades:

- `/Modelos/DTOs/Usuarios/` -> Carpeta destinada para los DTOs de Usuario.
  - `UsuarioCreateDto.cs`
  - `UsuarioUpdateDto.cs`
  - `UsuarioResponseDto.cs`
- `/Interfaces/`
  - `IUsuarioService.cs` (Actualización de Contratos)
- `/Services/`
  - `UsuarioService.cs` (Implementación de Mapeos y Lógica de Negocio)
- `/Pages/Usuarios/`
  - Modificación de los Page Models (`Create.cshtml.cs`, `Edit.cshtml.cs`, `Index.cshtml.cs`, `Details.cshtml.cs`) para utilizar puramente los DTOs.

---

## C. Contratos y Especificaciones

### 1. Data Transfer Objects (DTOs)

Se deben crear modelos anémicos dedicados a la transferencia de datos para evitar vulnerabilidades.

**UsuarioCreateDto.cs**
```csharp
using System.ComponentModel.DataAnnotations;

namespace ProyectoMSD.Modelos.DTOs.Usuarios
{
    public class UsuarioCreateDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "Se necesita un correo")]
        [EmailAddress(ErrorMessage = "Formato de correo inválido")]
        public string Correo { get; set; } = null!;

        [Required(ErrorMessage = "La clave es obligatoria para nuevos usuarios")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "La clave debe tener al menos 6 caracteres")]
        public string Clave { get; set; } = null!;

        [Required(ErrorMessage = "Especifica un Rol")]
        public string Rol { get; set; } = null!;

        public string? PrefijoTelefono { get; set; }
        public string? Telefono { get; set; }
        public string? Documento { get; set; }
        public string? Rut { get; set; }
        public int? IdUbicacion { get; set; }
    }
}
```

**UsuarioUpdateDto.cs**
```csharp
using System.ComponentModel.DataAnnotations;

namespace ProyectoMSD.Modelos.DTOs.Usuarios
{
    public class UsuarioUpdateDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; } = null!;

        [Required(ErrorMessage = "Se necesita un correo")]
        [EmailAddress]
        public string Correo { get; set; } = null!;

        public string? Rol { get; set; }
        public string? PrefijoTelefono { get; set; }
        public string? Telefono { get; set; }
        public string? Documento { get; set; }
        public string? Acesso { get; set; } // Permitido / Denegado, etc.
        public int? IdUbicacion { get; set; }
        
        // Propiedad opcional para resetear contraseña desde Admin
        [StringLength(50, MinimumLength = 6)]
        public string? NuevaClave { get; set; } 
    }
}
```

**UsuarioResponseDto.cs**
```csharp
namespace ProyectoMSD.Modelos.DTOs.Usuarios
{
    public class UsuarioResponseDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Correo { get; set; } = null!;
        public string Rol { get; set; } = null!;
        public string? Telefono { get; set; }
        public string? Acesso { get; set; }
        public string? Documento { get; set; }
        public int? IdUbicacion { get; set; }
    }
}
```

### 2. Actualización de Interface: `IUsuarioService`

La interfaz existente debe actualizar sus firmas para recibir y retornar DTOs, no las entidades del EF Core (eliminando dependencias de EF Core indirectas hacia las vistas).

```csharp
// CRUD de Usuarios Refactorizado a DTOs
Task<List<UsuarioResponseDto>> GetAllUsuariosAsync();
Task<UsuarioResponseDto?> GetUsuarioByIdAsync(int id);
Task<UsuarioUpdateDto?> GetUsuarioForEditAsync(int id);
Task CreateUsuarioAsync(UsuarioCreateDto dto);
Task UpdateUsuarioAsync(UsuarioUpdateDto dto);
```

---

## D. Integración y Pipeline

1. **Inyección de Dependencias Reafirmada**: El servicio `IUsuarioService` ya está inyectado como un contenedor `Scoped` en `Program.cs`. Esto se mantiene (ej. `builder.Services.AddScoped<IUsuarioService, UsuarioService>();`).
2. **Mapeo**: Dentro de `UsuarioService.cs`, utilizar mapeo manual seguro (o AutoMapper si el proyecto lo contempla) para transformar:
   - `UsuarioCreateDto` -> `Usuario`
   - `UsuarioUpdateDto` -> `Usuario` (Al recuperar desde DB y actualizar las propiedades mapeadas).
   - `Usuario` -> `UsuarioResponseDto`
3. **Seguridad**: Asegurar que las validaciones como contraseñas encriptadas sigan usando `_usuarioService.HashPassword(dto.Clave)` en la etapa de creación.

---

## E. Directrices Específicas para `@Dev-1.md`

Las siguientes son órdenes directas para el agente de ejecución, priorizando la consistencia, la mantenibilidad y la prevención de vulnerabilidades.

1. **Creación de DTOs**: Crea la carpeta `Modelos/DTOs/Usuarios` e implementa los tres DTOs definidos en la sección "Contratos".
2. **Refactorización de Interfaz**: Actualiza `IUsuarioService.cs` para reemplazar el uso de `Usuario` por los nuevos DTOs (`UsuarioCreateDto`, `UsuarioUpdateDto`, `UsuarioResponseDto`) en las operaciones estándar de lectura, creación y actualización.
3. **Refactorización de Servicio**: En `UsuarioService.cs`, implementa la lógica de mapeo entre los DTOs y el modelo de entidad subyacente. Asegúrate de solo mapear los campos declarados explícitamente y mantén la lógica de hash (encriptación) puramente contenida en el servicio.
4. **Desacoplamiento de las Vistas (Pages)**:
   - En `Pages/Usuarios/Create.cshtml.cs`, cambia `[BindProperty] public Usuario Usuario` a `[BindProperty] public UsuarioCreateDto Input`. Transmite `Input` al servicio para su creación.
   - En `Pages/Usuarios/Edit.cshtml.cs`, cambia a `[BindProperty] public UsuarioUpdateDto Input`. Carga el estado usando `GetUsuarioForEditAsync` y guárdalo usando `UpdateUsuarioAsync`.
   - Modifica los archivos `.cshtml` para reflejar visualmente estos cambios de modelo (`@model.Input.Nombre` en lugar de `@model.Usuario.Nombre`).
5. **Auditoría de Inyección (Anti-Pattern check)**: Si durante la refactorización se evidencia en algún punto (vistas/controladores) una inyección de `AppDbContext` relacionada con `Usuarios`, elimínala inmediatamente y delega esa responsabilidad al `UsuarioService` según SoC.

---
**Veredicto Arquitectónico (Tech Lead):**
Se elige mantener una arquitectura N-Tier clásica para este alcance, implementando DTOs rígidos entre la Capa de Presentación (Razor Pages) y la Capa de Negocio (Services). 
**Matriz de Decisión (Falta de DTOs vs Uso de DTOs):**
- *Rendimiento*: Neutral. El mapeo añade un costo marginal imperceptible.
- *Mantenibilidad*: Altamente superior al emplear DTOs. El esquema de DB `Usuario` queda protegido de vulnerabilidades *over-posting* a nivel UI.
- *Complejidad*: Media-baja. Obliga a escribir código extra para "Mapeos", pero previene errores catastróficos en el mediano plazo.
*El uso de DTOs es obligatorio.*
