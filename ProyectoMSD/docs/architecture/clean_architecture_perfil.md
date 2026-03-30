# Clean Architecture - Módulo del Perfil

El módulo del Perfil implementa el patrón de Arquitectura Limpia (**Clean Architecture**), buscando un desacoplamiento entre las vistas Razor Pages y el acceso a la Base de Datos (`AppDbContext`).

## Flujo de Datos

```mermaid
graph TD
    A[Perfil/Index.cshtml] -->|HTTP GET/POST| B(IndexModel PageModel)
    B -->|Inyección de Dependencia| C{IUsuarioService}
    C -->|Llamada GetUsuarioPerfilAsync| D[UsuarioService]
    C -->|Llamada UpdatePerfilAsync| D
    D -->|EF Core .Include() / CRUD| E[(Base de Datos MySQL)]
    D -->|Hash Contraseñas| F[PBKDF2 Cryptography]
    D -->|Crea Notificaciones| E
```

### Ventajas Obtenidas
1. **Separación de Responsabilidades:** El `PageModel` ya no contiene lógica condicional respecto a las coordenadas geográficas, hasheo de claves o creaciones manuales de entidades de notificaciones.
2. **Seguridad (Anti-Smart UI):** El `AppDbContext` deja de estar expuesto a la capa de UI.
3. **Mantenibilidad:** `IUsuarioService` es altamente propenso para la implementación de pruebas unitarias al abstraer la infraestructura (Base de datos MySQL).
