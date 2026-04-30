# Arquitectura y Módulos del Proyecto: MySmartDevice

Esta documentación detalla la estructura modular del sistema MySmartDevice (ProyectoMSD), el cual se basa en principios de **Clean Architecture / Arquitectura N-Capas**, promoviendo un bajo acoplamiento, alta cohesión y despliegues orientados a la nube.

## 1. Modelos (Entidades de Dominio)
**Ruta:** `/Modelos`
Representan la estructura de la base de datos relacional y son mapeados mediante **Entity Framework Core**.
- **`AppDbContext`**: El contexto de la base de datos que administra las entidades y la configuración de resiliencia (conexión a Aiven/MySQL).
- **Entidades Clave**: `Usuario`, `Propiedad`, `Espacio`, `Dispositivo`, `Notificacion`, `RegistroAcceso`, `Soporte`.
- **Regla Estricta**: Estas entidades **NO** deben enviarse directamente a la capa de presentación (Razor Pages) para evitar fugas de información y el anti-patrón de "Over-posting".

## 2. Modelos de Transferencia (DTOs)
**Ruta:** `/Modelos/DTOs`
Los DTOs (Data Transfer Objects) son la unidad de intercambio de información entre las distintas capas.
- **Propósito**: Moldear los datos según los requerimientos de la UI o API sin exponer la estructura de la base de datos subyacente.
- **Ejemplos Relevantes**: 
  - `DashboardMetricsDto` y `ClienteMetricasDto`: Agregación de datos (Big Data) para analíticas del Dashboard.
  - `ComandoEstructuradoDto`: Estructura estandarizada para el envío de instrucciones por MQTT.
  - `PagedResultDto`: Contenedor genérico para la paginación de listas en el lado del servidor.

## 3. Interfaces (Contratos)
**Ruta:** `/Interfaces`
Definen el contrato de comportamiento y las capacidades operativas de cada servicio en el sistema. Son esenciales para garantizar la Inversión de Dependencias (DI).
- **`IUsuarioService`, `IPropiedadService`, `IDispositivoService`**: Contratos para operaciones de gestión, administración y lógicas especializadas.
- **`IMqttPublisherService`**: Contrato enfocado exclusivamente en la emisión de comandos hacia la infraestructura IoT.
- **`IDashboardService`**: Contrato para la abstracción de operaciones de consulta analítica complejas.

## 4. Servicios (Lógica de Negocio)
**Ruta:** `/Services`
Contienen la inteligencia de negocio y orquestan la comunicación entre la base de datos, integraciones externas (Broker MQTT) y el frontend. **Aquí reside el núcleo de procesamiento del aplicativo.**
- **Servicios Scoped (Ciclo de vida por petición HTTP)**:
  - `DashboardService`: Agrupa consultas profundas con EF Core para generar KPIs y reportes para administradores y clientes.
  - `DispositivoService`: Gestiona la administración del hardware IoT, la creación de interfaces dinámicas y mapeo de capacidades lógicas.
  - `UsuarioService`, `SoporteService`: Procesan reglas de negocio, autorizaciones y manipulaciones de datos antes de solicitar la persistencia.
- **Servicios Singleton (Tareas en segundo plano)**:
  - `MqttDomoticaService`: (BackgroundService) Mantiene la conexión permanente y resiliente con el broker HiveMQ Cloud, recibiendo telemetría en tiempo real, registrando notificaciones del sistema y retransmitiendo paquetes al Frontend.

## 5. Controladores API Rest
**Ruta:** `/Controllers/Api`
Endpoints ligeros sin estado que exponen funciones específicas para consumo asíncrono vía AJAX o clientes IoT.
- **`MqttComandosController`**: Actúa como un middleware HTTP. Recibe intenciones de acciones desde el frontend y las canaliza hacia `IMqttPublisherService` para emitir instrucciones al microcontrolador (ESP32). Garantiza que las credenciales MQTT y la lógica de publicación nunca se expongan al navegador.

## 6. Hubs (Comunicación Bidireccional en Tiempo Real)
**Ruta:** `/Hubs`
Utiliza tecnología SignalR (WebSockets) propia del ecosistema ASP.NET Core.
- **`DispositivoHub`**: Mantiene un canal bidireccional seguro entre el servidor y las interfaces web activas de los usuarios. Recibe eventos del `MqttDomoticaService` y empuja las actualizaciones ("Push") hacia el navegador, refrescando la UI al instante sin recargas de página.

## 7. Pages (Capa de Presentación UI)
**Ruta:** `/Pages`
Implementado mediante ASP.NET Core Razor Pages con un enfoque fuerte en estética "Glassmorphism", diseño responsivo y moderno.
- **Regla Estricta**: Los archivos de código subyacente (PageModels) actúan estrictamente como orquestadores ("Thin Controllers"). Inyectan las interfaces (`IServices`) requeridas, construyen `DTOs` e interactúan con el backend sin incluir en ninguna circunstancia lógica de conexión a datos.
