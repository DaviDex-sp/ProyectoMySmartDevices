# Documentación Técnica y Funcional: ProyectoMSD (MySmartDevice)

Este documento ha sido diseñado para proporcionar a cualquier persona ajena (o nueva) al aplicativo una comprensión integral, detallada y "página por página" de cómo está compuesto el proyecto **ProyectoMSD (MySmartDevice)**, las metodologías implementadas y sus funciones principales.

---

## 1. Visión General y Propósito
**MySmartDevice** es una plataforma web enfocada en la gestión domótica y administración inmobiliaria. Permite a los usuarios gestionar sus **Propiedades** y **Espacios** (habitaciones, oficinas, etc.), así como conectar y administrar **Dispositivos** inteligentes. Además, incluye módulos administrativos para visualizar métricas, administrar usuarios y gestionar tickets de soporte técnico.

---

## 2. Stack Tecnológico y Metodología

### Tecnologías Utama:
*   **Framework Principal:** ASP.NET Core 9.0
*   **Paradigma de UI:** Razor Pages (modelo `PageModel` y archivos `.cshtml` para renderizado del lado del servidor).
*   **Lenguaje:** C# (C Sharp).
*   **ORM (Acceso a Datos):** Entity Framework Core 9 (`Microsoft.EntityFrameworkCore.Relational`).
*   **Base de Datos:** MySQL 8.0 gestionado mediante el proveedor `Pomelo.EntityFrameworkCore.MySql`.
*   **Autenticación:** Sistema de identidad híbrido usando Cookies locales (`CookieAuthenticationDefaults`) y autenticación externa (OAuth) con Google (`Microsoft.AspNetCore.Authentication.Google`).

### Metodología de Desarrollo:
1.  **Arquitectura Limpia (Separación de Responsabilidades):** El proyecto sigue una arquitectura clásica monolítica de N-Capas, dividiendo responsabilidades en:
    *   **Capa de Presentación:** Archivos dentro de `/Pages` (HTML + C# embebido).
    *   **Capa Lógica (Servicios):** Implementaciones dentro de `/Services` bajo contratos (Interfaces) ubicadas en `/Interfaces`. La inyección de dependencias (`DI`) se orquesta en el `Program.cs` usando `AddScoped()`.
    *   **Capa de Datos:** Clases en `/Modelos` mapeadas por un `AppDbContext` utilizando una aproximación Code-First/DB-First configuradas con la Fluent API.
2.  **Inyección de Dependencias (DI):** Totalmente adoptada. Los servicios como `UsuarioService` o `DashboardService` son inyectados en los constructores de cada `PageModel`.
3.  **Seguridad Base:** Algoritmos de "Hashing" (PBKDF2) para contraseñas en la base de datos y autorización a nivel de página o carpeta mediante filtros o comprobaciones de `HttpContext.User`.

---

## 3. Arquitectura y Estructura Organizativa
A nivel de sistema de archivos, el proyecto sigue esta estructura:

*   `/Pages`: Contiene las vistas (UI) y su lógica de respaldo (`.cshtml.cs`). Está agrupado funcionalmente por carpetas asociadas a las entidades.
*   `/Modelos`: Clases "Poco" (Plain Old C# Objects) que representan las tablas de la base de datos (Ej: `Usuario`, `Dispositivo`, `Propiedad`). Aquí se ubica el `AppDbContext.cs`.
*   `/Services`: Lógica de negocio dura e interacciones complejas con la base de datos (Ej. Hashing, consolidación de métricas).
*   `/Interfaces`: Contratos que garantizan el Polimorfismo y facilitan las pruebas unitarias/mocking de los Servicios (Ej: `IUsuarioService`).
*   `/Filters`: Interceptores HTTP personalizados, como `NotificacionNavbarFilter`, que inyectan datos globales en la sesión de forma transparente antes de renderizar la página.
*   `/wwwroot`: Archivos estáticos públicos (CSS, JS, imágenes).
*   `Program.cs`: Configuraciones de inicio, Inyección de Dependencias (Service Container), DbContext y Middleware.
*   `appsettings.json`: Configuración en crudo (cadenas de conexión a MySQL, credenciales de las APIs de Google). Se usa infraestructura en la nube (ej. Azure/Aiven).

---

## 4. Base de Datos: Modelos Relacionales (AppDbContext)
El ecosistema maneja las siguientes entidades fundamentales mapeadas explícitamente usando la Fluent API:

1.  **Usuarios:** Guarda los datos del cliente/admin, correos, hash de contraseña y rol.
2.  **Propiedades:** Vinculadas a los usuarios (Ej. "Casa de Campo", "Oficina"). Dependen directamente del Usuario.
3.  **Espacios:** Zonas físicas dentro de una propiedad (Ej. "Sala", "Cuarto Principal"). Tienen relación obligatoria con `Propiedades`.
4.  **Dispositivos:** Representan aparatos inteligentes genéricos almacenados en el sistema (Ej. Foco Smart, TV).
5.  **Almacenan:** Es una relación N a N. Define qué *Dispositivo* está ubicado en qué *Espacio* físico.
6.  **Configuraciones:** Ajustes a nivel de software asignadas a un usuario y dispositivo (idiomas, alertas).
7.  **Soportes:** Sistema de Tickets (Petición/Queja/Reclamo) iniciados por Usuarios.
8.  **Notificaciones:** Alertas persistentes del sistema vinculadas a los Usuarios para avisos asincrónicos.
9.  **RegistroAccesos:** Datos de telemetría y auditoría de sesiones (IP, Navegador, Acción, Duración).

---

## 5. Descripción Detallada: Página por Página (`/Pages/`)

Aquí el flujo UI detallado de cómo interactúa el usuario y cada módulo dentro de la plataforma:

### Raíz (Paginación Abierta y Autenticación)
*   **`Index.cshtml`:** Punto de entrada o "Landing Page". Verifica la autenticación; si el usuario está logueado, lo desvía inmediatamente al dashboard o lista de usuarios, en su defecto muestra la presentación.
*   **`Login.cshtml`:** Página de Identificación. Procesa credenciales manuales y contiene el botón de inicio con Google (OAuth). Si los datos son correctos, expide la Cookie cifrada de sesión.
*   **`Registrar.cshtml`:** Proceso de afiliación. Realiza verificaciones de existencia previas (correo/documento) e invoca algoritmos de Hashing para guardar de forma segura al usuario.
*   **`Logout.cshtml`:** Cierre de Sesión seguro. Invalida la autenticación de la cookie y guarda una métrica en los logs calculando la duración total de la sesión.
*   **`Denegada.cshtml` / `Error.cshtml`:** Páginas de estado interactivas y amigables. Fallos generales y errores por falta de permisos de roles.

### `Dashboard/`
*   **`Index`:** El motor de analíticas administrativas. Inyecta métricas como tráfico diario (Logs de 7 días), ingresos por mes, cantidad de tickets de soporte no resueltos, gráficos de donas / barras con el top de ciudades o navegadores, traídos a través del potente DTO de `DashboardService`.

### `Usuarios/`
*   Contiene un esquema clásico de CRUD (Create, Read, Update, Delete) + Vistas complementarias.
*   Páginas subyacentes: `Index` (Tabla con filtros y buscadores), `Create`, `Edit`, `Details`, `Delete`. Administra roles, niveles de acceso y estados de las cuentas.

### `Perfil/`
*   **`Index`:** Le permite al usuario activo modificar su información personal sin interferir con variables críticas (ej. roles o nivel de seguridad).
    *   **Arquitectura Limpia Aplicada:** El PageModel (`Index.cshtml.cs`) está totalmente desacoplado del `AppDbContext`. Toda la lógica de negocio (hasheo de contraseñas, inserción de notificaciones y gestión de ubicación) se delegó a la capa de servicios mediante `IUsuarioService` (métodos `GetUsuarioPerfilAsync` y `UpdatePerfilAsync`), cumpliendo con el patrón Anti-Smart UI.

### Gestión Inmobiliaria (`Propiedades/` y `Espacios/`)
*   Páginas especializadas que permiten al Propietario o Inversionista modelar de manera virtual su infraestructura física (Ej. Registrar un piso, luego asignarle una habitación llamada "Baño Principal"). Manejan CRUD estándar atados a los ID de las capas superiores o perfiles.

### Gestión Domótica (`Dispositivos/` y `Almacenan/`)
*   Permite cargar en el sistema toda serie de gadgets tecnológicos (`Dispositivos`) para posteriormente, con el esquema relacional intermedio `Almacenan`, emparejar que el "Foco Smart A" estará funcionado bajo la red Wi-Fi del espacio "Habitación 1".
*   **[NUEVO] Telemetría y Control IoT Bidireccional (SignalR):** La página de detalles de dispositivos usa WebSockets (SignalR `DispositivosHub`) para graficar telemetría en Tiempo Real de Temperatura y Humedad provenientes del sensor (Vía HiveMQ). Simultáneamente, orquesta los botones de apagado enviando peticiones POST al Controlador `MqttComandosController`, consolidando tanto un control remoto asíncrono como lectura instantánea.

### Control de Fallos y Soporte técnico (`Soportes/`)
*   Sección destinada al contacto rápido y tickets de soporte. Los usuarios crean tickets (Ej. "Falla conexión BD de panel LED"), y posteriormente el Administrador visualizará aquellos donde la columna `Respuesta` se encuentre vacía (KPI) para gestionarlos activamente.

### Interacciones Silenciosas (`Notificaciones/`)
*   Panel tipo buzón. Funciona en acompañamiento con el filtro global `NotificacionNavbarFilter` y el modelo `Notificacion` para alertarle al usuario si un ticket falló, si su perfil fue validado o informar promociones. 

---

## 6. Funciones Más Usadas (La "Core Logic")

Para un entendimiento rápido, a continuación las funciones operacionales clave sobre las que cabalga toda la arquitectura de la web (Principalmente localizadas en el directorio `Services/`):

1.  **`AuthenticateAsync(correo, password)` y `AuthenticateGoogleAsync()` (en `UsuarioService.cs`):** 
    Son las encargadas de validar accesos. La primera encripta la contraseña proveída y la coteja en la base de datos de Entity Framework. La segunda (Google) crea automáticamente una cuenta "Shadow" o por defecto (si el usuario de google no existe previamente) garantizando fricción cero al registro.

2.  **`HashPassword(password)` (en `UsuarioService.cs`):** 
    Método de seguridad que implementa `PBKDF2` con `Rfc2898DeriveBytes`. Genera un Salt dinámicamente y expide la contraseña concatenada en fomato Base64, evitando que un acceso a la Base de datos comprometa la seguridad del usuario.

3.  **`RegisterAccessAsync()` y `RegisterLogoutAsync()`:** 
    Auditoría extrema. Estas funciones de telemetría inyectan en segundo plano logs dentro de la tabla `RegistroAccesos`. Miden tiempos exactos de sesión (restación del `DateTime.Now`), la User-Agent del navegador y su dirección IP con fines de ciberseguridad y Data-Mining para el Dashboard.

4.  **`GetMetricsAsync()` (en `DashboardService.cs`):** 
    Retorna un masivo `DashboardMetricsDto`. Encapsula múltiples consultas asincronas a EF (.CountAsync(), .Average(), agrupaciones relacionales). Calcula Demografía en servidor para liberar carga al cliente, y es la encargada de nutrir librerías visuales tipo `Chart.js` que luego despliega la vista del Admin.

5.  **`PublishCommandAsync()` (en `MqttPublisherService.cs`) y `IHubContext`:** 
    Es el core del control Cloud-Native IoT. Separan las responsabilidades bidireccionales donde las instrucciones de interfaz de usuario hacia la máquina se publican (Push/Write) en su propio sub-proceso, mientras que el Background Service escucha asincrónicamente inyectando WebSockets en los clientes subscritos a un dispositivo.

---
**NOTA DE SEGURIDAD (Regla 1 y 3 del desarrollador):** El entorno actual inyecta directamente credenciales sensibles vinculadas a Azure, SendGrid o APIs. La configuración del entorno (.json) nunca debe ser sincronizada a repositorios como GitHub por estrictas medidas de seguridad implantadas en el pipeline.
