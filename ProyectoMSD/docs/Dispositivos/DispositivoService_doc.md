# DispositivoService - Documentacion del Modulo

## Proposito

DispositivoService gestiona todas las operaciones de acceso a datos de la entidad Dispositivo. Expone una interfaz limpia basada en DTOs hacia los PageModels y Controladores, manteniendo la capa de persistencia completamente oculta de la interfaz de usuario.

Actualizacion en esta version: se anio el metodo CreateAsync que persiste el nuevo Dispositivo en la base de datos y genera automaticamente una notificacion de tipo configuracion para el usuario autenticado que realizo el registro, invocando a INotificacionService.

---

## Dependencias

| Dependencia | Tipo | Ciclo de vida | Proposito |
|---|---|---|---|
| AppDbContext | EF Core DbContext | Scoped | Operaciones CRUD sobre dispositivos |
| INotificacionService | Interfaz de servicio | Scoped | Emision de notificacion post-creacion |

Registro en DI (Program.cs):

    // INotificacionService DEBE registrarse ANTES que IDispositivoService
    builder.Services.AddScoped<INotificacionService, NotificacionService>();
    builder.Services.AddScoped<IDispositivoService, DispositivoService>();

---

## API Publica / Interfaz

### GetDispositivosAsync()

Retorna todos los dispositivos mapeados a DispositivoDto, incluyendo helpers de UI calculados (IconClass, BadgeClass, IsActive).

### GetByIdAsync(int id)

Retorna un unico DTO de dispositivo por ID, o null si no existe.

### GetTotalDispositivosAsync()

Retorna el conteo total de dispositivos registrados. Usado por el Dashboard de metricas.

### ToggleEstadoAsync(int id)

Alterna el campo Estado entre Activo e Inactivo. Retorna false si el dispositivo no existe o si ocurre un error en la base de datos.

### UpdateAsync(DispositivoDto dto)

Actualiza todos los campos modificables de un dispositivo existente a partir de un DTO. Retorna false en caso de fallo.

### CreateAsync(Dispositivo dispositivo, int idUsuarioCreador) - NUEVO

| Parametro | Tipo | Descripcion |
|---|---|---|
| dispositivo | Dispositivo | Entidad con todos los campos requeridos completados |
| idUsuarioCreador | int | ID del usuario autenticado que ejecuta el registro |

Retorna: el ID del dispositivo recien creado.

Comportamiento:
1. Ejecuta _context.Dispositivos.Add(dispositivo) + SaveChangesAsync().
2. Tras el guardado exitoso, invoca _notificacionService.CrearAsync con tipo configuracion y ruta de redireccion a la pagina de detalles del dispositivo.

---

## Ejemplo de Uso

    // En el PageModel Create.cshtml.cs
    var userIdClaim = User.FindFirstValue("UserId");
    int idUsuario   = int.TryParse(userIdClaim, out int uid) ? uid : 0;

    await _dispositivoService.CreateAsync(Dispositivo, idUsuario);
    return RedirectToPage("./Index");

---

## Notas Tecnicas

- idUsuarioCreador toma el valor 0 si el claim no esta presente. Esto no deberia ocurrir gracias al filtro Authorize aplicado en el PageModel.
- GetDispositivosAsync y GetByIdAsync usan AsNoTracking() para optimizar el rendimiento en consultas de solo lectura.
- Los helpers de UI (IsActive, IconClass, BadgeClass) se calculan en metodos privados para mantener el DTO limpio y libre de logica.
- El orden de registro en DI es obligatorio: INotificacionService debe registrarse antes que IDispositivoService ya que es dependencia de su constructor.
