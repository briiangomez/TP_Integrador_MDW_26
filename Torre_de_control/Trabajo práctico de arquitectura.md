# Trabajo práctico
## Modelos computacionales de gestión administrativa

**Profesor:** Sebastián Nicolás Luna  
**Correo:** sebastian.luna@uai.edu.ar
## Contenido
Objetivo ...................................................................................................................................................... 3
Estrategias de Aprendizaje ....................................................................................................................... 3
Modalidad de trabajo ............................................................................................................................... 3
Tecnologías................................................................................................................................................. 3
   Obligatorias (stack base) ....................................................................................................................... 3
   Alternativas / a elección de cada grupo ............................................................................................ 4
Arquitectura general ................................................................................................................................. 4
   Arquitectura de la solución ................................................................................................................... 5
Módulos por grupo .................................................................................................................................... 6
## Grupo 1 — Torre de Control................................................................................................................... 6
      Responsabilidad .................................................................................................................................. 6
      Ejemplos de flujo ................................................................................................................................. 6
      Configuraciones de negocio necesarias ......................................................................................... 7
## Grupo 2 — Gestión de Vuelos ............................................................................................................... 7
      Responsabilidad .................................................................................................................................. 7
      Ejemplos de flujo ................................................................................................................................. 7
      Configuraciones de negocio necesarias ......................................................................................... 8
## Grupo 3 — Pistas y Aeronaves............................................................................................................... 8
      Responsabilidad .................................................................................................................................. 8
      Ejemplos de flujo ................................................................................................................................. 8
      Configuraciones de negocio necesarias ......................................................................................... 9
## Grupo 4 — Check-in / Embarque ......................................................................................................... 9
      Responsabilidad .................................................................................................................................. 9
      Ejemplos de flujo ................................................................................................................................. 9
      Configuraciones de negocio necesarias ....................................................................................... 10
## Grupo 5 — Equipaje ............................................................................................................................. 10
      Responsabilidad ................................................................................................................................ 10
      Ejemplos de flujo ............................................................................................................................... 10
      Configuraciones de negocio necesarias ....................................................................................... 10
## Grupo 6 — Meteorología + Infraestructura compartida................................................................... 11
      Responsabilidad ................................................................................................................................ 11
      Ejemplos de flujo ............................................................................................................................... 11
      Configuraciones de negocio necesarias ....................................................................................... 11
## Grupo 7 — Seguridad y Usuarios ......................................................................................................... 12
      Responsabilidad ................................................................................................................................ 12
      Ejemplos de flujo ............................................................................................................................... 12
      Configuraciones de negocio necesarias ....................................................................................... 12
## Grupo 8 — Logs / Notificaciones y Pantallas ..................................................................................... 13
      Responsabilidad ................................................................................................................................ 13
      Ejemplos de flujo ............................................................................................................................... 13
      Configuraciones de negocio necesarias ....................................................................................... 13
## Grupo 9 — Panel de Administración .................................................................................................. 14
      Responsabilidad ................................................................................................................................ 14
      Ejemplos de flujo ............................................................................................................................... 14
      Configuraciones de negocio necesarias ....................................................................................... 14
Infraestructura compartida ..................................................................................................................... 15
Requerimientos no funcionales............................................................................................................... 15
Contratos técnicos................................................................................................................................... 16
   Token JWT (emitido por Seguridad — Grupo 7) ................................................................................. 16
   Envelope estándar de eventos del bus (RabbitMQ)......................................................................... 16
   Ejemplo de contrato REST — Solicitud de autorización (Vuelos → Torre de Control) ..................... 17
      Request .............................................................................................................................................. 17
      Response............................................................................................................................................ 17
Entregables ............................................................................................................................................... 18
   Documentación ................................................................................................................................... 18
   Código fuente ...................................................................................................................................... 18
Criterio de evaluación ............................................................................................................................. 18
## Objetivo
Brindar el espacio para aplicar, de forma integrada, los conocimientos de diseño de software,
arquitectura distribuida y desarrollo de sistemas en red. El trabajo abarca el análisis del problema,
el diseño de la solución propuesta, la implementación y la interconexión obligatoria con servicios
desarrollados por otros grupos, simulando así un entorno real de integración entre equipos
independientes.
## Estrategias de Aprendizaje
Se espera que los estudiantes formen grupos y pongan en práctica los conocimientos adquiridos
en cada lección.

Cada grupo deberá dar una solución tecnológica a los requerimientos planteados por el
profesor, con el fin de desarrollar las habilidades analíticas para el diseño y solución de
problemas que conllevan la implementación de algoritmos.
## Modalidad de trabajo
- El tema es único para todo el curso: un sistema de gestión aeroportuaria.
- El curso se organiza en 9 grupos, cada uno responsable de un componente/módulo
       distinto del sistema.
- Cada grupo diseña e implementa su módulo de forma autónoma, pero debe
       obligatoriamente interconectarse con uno o más módulos de otros grupos, combinando
       distintos tipos de comunicación (REST, service bus, cliente-servidor TCP/UDP).
- El Grupo 6 (Meteorología) es además responsable de montar y mantener la infraestructura
       compartida (ver sección 6).
## Tecnologías
### Obligatorias (stack base)
- C# / .NET
- JavaScript, HTML, CSS
- Docker
- RabbitMQ
- REST API
- TCP y/o UDP (para al menos una interconexión de tipo cliente-servidor)
### Alternativas / a elección de cada grupo
- React, Vue o Angular (frontend)
- ASP.NET Web Forms / MVC
- SQL Server o MongoDB (persistencia — cada grupo puede elegir el motor más adecuado
       a su tipo de dato)
- ZeroMQ
- Librerías de logging estructurado

Cada grupo puede incorporar tecnologías adicionales al stack base, siempre que se justifiquen
en la documentación.
## Arquitectura general
Los clientes web (Panel de Administración, Pantallas Públicas, Check-in/Embarque) acceden a
los microservicios a través de un API Gateway / Reverse Proxy, que actúa como punto único de
entrada. Los microservicios se comunican entre sí de tres formas posibles:
- REST para consultas y operaciones síncronas servicio a servicio.
- RabbitMQ (service bus) para eventos asíncronos (cambios de estado, notificaciones, logs
       centralizados).
- TCP/UDP para comunicación cliente-servidor de baja latencia (ej. telemetría de posición
       de aeronaves).

Cada servicio administra su propia base de datos (persistencia poliglota: SQL Server o MongoDB
según convenga).

Ver diagrama de arquitectura adjunto (arquitectura-tp-aeropuerto.mermaid) con el detalle de
conexiones, gateway, bus y bases de datos.
### Arquitectura de la solución
## Módulos por grupo
## Grupo 1 — Torre de Control
### Responsabilidad
Recibir solicitudes de aterrizaje/despegue, arbitrar el uso de pistas y emitir autorizaciones.
- Requerimientos funcionales:
- Recibir solicitudes de autorización de despegue/aterrizaje (REST) desde Gestión de
              Vuelos.
- Recibir telemetría de posición de aeronaves (UDP) desde Pistas y Aeronaves.
- Recibir alertas meteorológicas (TCP) desde Meteorología y bloquear autorizaciones
              si corresponde.
- Publicar eventos de autorización otorgada/denegada al service bus.
- Exponer API de configuración de reglas (mínimos de separación entre operaciones)
              consumida por el Panel de Administración, validando el token JWT emitido por
              Seguridad.
- Interconexiones: Gestión de Vuelos (REST, entrante), Pistas y Aeronaves (UDP, entrante),
       Meteorología (TCP, entrante), Panel de Administración (REST, entrante), Seguridad
       (validación de token, ver sección 7), RabbitMQ (saliente).
- Persistencia: base de datos propia con estado de pistas y autorizaciones. Esta es la base
       de datos "principal" del sistema — su infraestructura es montada por el grupo de
       infraestructura (sección 6), aunque el esquema y la lógica de acceso son responsabilidad
       de este grupo.
### Ejemplos de flujo
   1. Autorización de despegue

          1. Gestión de Vuelos (Grupo 2) envía una solicitud REST con vuelo_id y pista solicitada.

          2. Torre de Control verifica que la pista esté libre y que no haya una alerta
             meteorológica activa.

          3. Si está todo en orden, otorga la autorización, la persiste y publica el evento
             AutorizacionOtorgada al bus.

          4. Si no, deniega la solicitud y publica AutorizacionDenegada indicando el motivo.

   2. Bloqueo de pista por alerta climática

          1. Meteorología (Grupo 6) envía una alerta por TCP (ej. "viento cruzado excede el
             límite").

          2. Torre de Control marca la pista afectada como no operativa.

          3. Publica el evento PistaBloqueada al bus (lo consumen Logs y Pantallas Públicas —
             Grupo 8).

          4. Mientras dure el bloqueo, cualquier solicitud de autorización sobre esa pista se
             deniega automáticamente.
### Configuraciones de negocio necesarias
- Separación mínima (en segundos) entre operaciones consecutivas por pista.
- Umbral de viento cruzado y visibilidad mínima para habilitar operaciones.
- Cantidad de pistas disponibles y su estado inicial.
## Grupo 2 — Gestión de Vuelos
### Responsabilidad
Administrar el ciclo de vida de cada vuelo.
- Requerimientos funcionales:
- CRUD de vuelos (origen, destino, horario, aeronave asignada).
- Máquina de estados del vuelo: programado → embarcando → rodando →
              despegando → en vuelo → aterrizando → arribado.
- Solicitar autorización a Torre de Control (REST) en los cambios de estado que lo
              requieran.
- Publicar cambios de estado al service bus.
- Exponer API pública de consulta de itinerarios (consumida por Pantallas Públicas vía
              Gateway).
- Interconexiones: Torre de Control (REST, saliente), RabbitMQ (saliente).
- Persistencia: base propia (vuelos, horarios, estados).
### Ejemplos de flujo
   1. Cambio de estado a "despegando"

          1. El vuelo alcanza el estado "listo para despegar" (embarque completo, notificado
             por Check-in — Grupo 4).

          2. Gestión de Vuelos solicita autorización a Torre de Control (Grupo 1, REST).

          3. Si es autorizado, actualiza el estado a "despegando" y luego a "en vuelo".

          4. Publica el evento EstadoVueloCambiado al bus.

   2. Alta de un nuevo vuelo

          1. Un operador crea un vuelo (origen, destino, horario, aeronave asignada).

          2. El servicio valida los datos y lo persiste.

          3. Publica el evento VueloCreado al bus.
### Configuraciones de negocio necesarias
- Tiempo mínimo requerido entre cierre de embarque y solicitud de despegue.
- Horarios de operación habilitados del aeropuerto.
- Catálogo de aeronaves y rutas disponibles.
## Grupo 3 — Pistas y Aeronaves
### Responsabilidad
Simular la posición y el estado de las aeronaves y la ocupación de pistas.
- Requerimientos funcionales:
- Simular telemetría de posición de aeronaves y enviarla a Torre de Control (UDP).
- Detectar y reportar ocupación/liberación de pista.
- Consumir del service bus las autorizaciones otorgadas por Torre de Control.
- Publicar eventos de cambio de estado de pista.
- Interconexiones: Torre de Control (UDP saliente, RabbitMQ entrante/saliente).
- Persistencia: base propia (histórico de posiciones/telemetría — recomendado MongoDB
       por volumen y estructura variable).
### Ejemplos de flujo
   1. Telemetría periódica

          1. El simulador de una aeronave envía un paquete UDP con posición y velocidad
             cada N segundos a Torre de Control (Grupo 1).

          2. Pistas y Aeronaves registra la posición en su base.

          3. Si detecta que un aterrizaje o despegue se completó, publica el evento
             correspondiente al bus.

   2. Detección de ocupación de pista

          1. Una aeronave inicia el procedimiento de aterrizaje.

          2. Pistas y Aeronaves marca la pista como "ocupada".

          3. Al liberarse, publica PistaLiberada al bus, consumido por Torre de Control (Grupo 1).

   3. Recepción de autorización vía bus

          1. Torre de Control (Grupo 1) publica el evento AutorizacionOtorgada al bus tras
             autorizar una operación.

          2. Pistas y Aeronaves consume el evento y habilita el escenario correspondiente en el
             simulador (ej. inicia la secuencia de despegue/aterrizaje de esa aeronave).
### Configuraciones de negocio necesarias
- Frecuencia de envío de telemetría (paquetes por segundo).
- Tiempo estimado de ocupación de pista por tipo de operación (despegue/aterrizaje).
- Catálogo de aeronaves simuladas y sus características.
## Grupo 4 — Check-in / Embarque
### Responsabilidad
Gestionar pasajeros, asientos y tarjetas de embarque.
- Requerimientos funcionales:
- Registro de pasajeros y asignación de asientos por vuelo.
- Emisión de tarjeta de embarque (simulada).
- Validar identidad/usuario contra Seguridad (REST).
- Consultar datos del vuelo contra Gestión de Vuelos (REST).
- Interconexiones: Seguridad (REST, saliente), Gestión de Vuelos (REST, saliente).
- Persistencia: base propia (pasajeros, asientos, embarque).
### Ejemplos de flujo
   1. Check-in de un pasajero

          1. El pasajero se identifica (login validado contra Seguridad — Grupo 7, REST).

          2. El sistema consulta los datos del vuelo a Gestión de Vuelos (Grupo 2, REST).

          3. Asigna un asiento disponible y genera la tarjeta de embarque.

          4. Queda disponible para que Equipaje (Grupo 5) lo consulte al despachar valijas.

   2. Cierre de embarque

          1. Se alcanza el horario límite de embarque del vuelo.

          2. El servicio notifica a Gestión de Vuelos (Grupo 2) que el embarque está completo.

          3. Gestión de Vuelos actualiza el estado a "listo para despegar".
### Configuraciones de negocio necesarias
- Tiempo límite de check-in respecto a la hora de salida del vuelo.
- Mapa de asientos por tipo de aeronave.
- Cantidad de puertas de embarque disponibles.
## Grupo 5 — Equipaje
### Responsabilidad
Registrar y trackear el equipaje despachado.
- Requerimientos funcionales:
- Vincular cada valija a un pasajero/vuelo, consultando a Check-in (REST).
- Registrar estados del equipaje (despachado, en tránsito, en bodega, retirado).
- Publicar eventos de equipaje al service bus (para logs y notificaciones).
- Interconexiones: Check-in/Embarque (REST, saliente), RabbitMQ (saliente).
- Persistencia: base propia (recomendado MongoDB).
### Ejemplos de flujo
   1. Despacho de valija

          1. El pasajero despacha su equipaje durante el check-in.

          2. Equipaje consulta a Check-in (Grupo 4, REST) los datos del pasajero y del vuelo.

          3. Genera la etiqueta y registra el estado inicial "despachado".

          4. Publica el evento correspondiente al bus.

   2. Seguimiento y entrega

          1. El equipaje cambia de estado a medida que avanza (en tránsito, en bodega,
             retirado), simulando su recorrido.

          2. Cada cambio de estado se publica al bus para trazabilidad en Logs (Grupo 8).
### Configuraciones de negocio necesarias
- Peso máximo permitido por pasajero y cantidad de valijas incluidas sin cargo.
- Tiempos estimados por etapa del recorrido (para la simulación).
## Grupo 6 — Meteorología + Infraestructura compartida
### Responsabilidad
Proveer condiciones climáticas simuladas que puedan afectar operaciones, y montar la
infraestructura común del sistema.
- Requerimientos funcionales (módulo):
- Simular condiciones climáticas (viento, visibilidad, tormentas) por franja horaria.
- Notificar alertas a Torre de Control (TCP) cuando las condiciones impidan operar.
- Responsabilidades de infraestructura (adicionales, por tener la menor carga funcional):
- Desplegar y configurar RabbitMQ (colas/tópicos acordados con todos los grupos).
- Desplegar y configurar el API Gateway.
- Montar la infraestructura de la base de datos "principal" (la de Torre de Control —
              sección 5, Grupo 1).
- Documentar el proceso de despliegue de la infraestructura compartida para el
              resto de los grupos.
- Interconexiones: Torre de Control (TCP, saliente).
- Persistencia: base propia (histórico climático).
### Ejemplos de flujo
   1. Generación de alerta

          1. El simulador genera condiciones climáticas periódicas (viento, visibilidad,
             tormentas).

          2. Si alguna condición supera el umbral configurado, se genera una alerta.

          3. Se envía la alerta a Torre de Control (Grupo 1) por TCP.

   2. Reporte periódico

          1. Cada N minutos se publica el estado climático actual (haya o no alerta) al bus.

          2. Logs/Pantallas Públicas (Grupo 8) lo reflejan como información general.
### Configuraciones de negocio necesarias
- Umbrales de viento, visibilidad y condiciones que constituyen una alerta.
- Frecuencia de actualización de las condiciones climáticas simuladas.
## Grupo 7 — Seguridad y Usuarios
### Responsabilidad
Autenticación y autorización de todos los paneles y APIs.
- Requerimientos funcionales:
- Gestión de roles (controlador aéreo, operador de tierra, administrador).
- Emisión y validación de tokens de acceso para usuarios humanos (login con
              credenciales).
- Registrar las API keys (client id/secret) que cada grupo-proveedor le entrega, para
              poder emitir tokens de servicio (client credentials) con el audience correspondiente
              cuando un grupo consumidor lo solicite.
- Exponer API de autenticación consumida por Check-in, Panel de Administración y
              Torre de Control.
- Interconexiones: expone REST consumido por Grupos 1, 4 y 9.
- Persistencia: base propia (usuarios, roles, credenciales).
### Ejemplos de flujo
   1. Login

          1. El usuario envía sus credenciales por REST.

          2. Seguridad las valida y genera un token con el rol asociado.

          3. Los servicios consumidores (Check-in — Grupo 4, Panel de Administración — Grupo
             9, Torre de Control — Grupo 1) validan ese token en cada request.

   2. Autorización por rol

          1. El Panel de Administración (Grupo 9) solicita acceso a una función crítica (ej.
             modificar reglas operativas).

          2. Seguridad verifica que el token corresponda a un rol con permiso (ej. administrador)
             antes de permitirlo.
### Configuraciones de negocio necesarias
- Roles disponibles y permisos asociados a cada uno.
- Tiempo de expiración de los tokens de sesión.
## Grupo 8 — Logs / Notificaciones y Pantallas
### Responsabilidad
Centralizar logs de todo el sistema y alimentar las pantallas públicas de información.
- Requerimientos funcionales:
- Consumir eventos de todos los grupos vía service bus.
- Persistir logs centralizados, identificando el componente de origen.
- Exponer API de consulta de logs (consumida por el Panel de Administración).
- Alimentar en tiempo real la información mostrada en Pantallas Públicas
              (salidas/llegadas).
- Interconexiones: RabbitMQ (entrante, de todos los grupos), Panel de Administración (REST,
       saliente).
- Persistencia: base propia (recomendado MongoDB por volumen de escritura).
### Ejemplos de flujo
   1. Consumo y centralización

          1. El servicio recibe eventos de todos los grupos a través del bus.

          2. Identifica el componente de origen y el tipo de evento.

          3. Lo persiste junto con un timestamp.

          4. Si el evento es de interés público (ej. cambio de estado de un vuelo), actualiza
             Pantallas Públicas en tiempo real.

   2. Consulta desde el panel

          1. El Panel de Administración (Grupo 9) solicita logs filtrados por fecha, componente o
             tipo (REST).

          2. Logs responde con el listado correspondiente.
### Configuraciones de negocio necesarias
- Qué tipos de evento se muestran en pantallas públicas y cuáles quedan solo como log
       interno.
- Tiempo de retención de los logs.
## Grupo 9 — Panel de Administración
### Responsabilidad
Interfaz gráfica de configuración y monitoreo para el operador.
- Requerimientos funcionales:
- Configurar reglas operativas (ej. mínimos de separación) consumidas por Torre de
              Control (REST).
- Visualizar estado general del sistema consultando Logs (REST).
- Autenticarse contra Seguridad (REST).
- Encender/apagar servicios simulados (según alcance que el grupo defina).
- Interconexiones: Torre de Control (REST, saliente), Logs (REST, saliente), Seguridad (REST,
       saliente).
- Persistencia: base propia (configuración).
### Ejemplos de flujo
   1. Configuración de reglas operativas

          1. Un administrador autenticado (vía Seguridad — Grupo 7) modifica un parámetro
             (ej. separación mínima entre despegues).

          2. El Panel envía el cambio a Torre de Control (Grupo 1, REST).

          3. Torre de Control actualiza su configuración interna.

   2. Monitoreo general

          1. El Panel consulta periódicamente a Logs (Grupo 8, REST) el estado general del
             sistema.

          2. Muestra un dashboard con métricas (autorizaciones otorgadas/denegadas, alertas
             activas, vuelos en curso, etc.).
### Configuraciones de negocio necesarias
- Qué métricas se muestran en el dashboard.
- Frecuencia de refresco de la información mostrada.
## Infraestructura compartida
El Grupo 6 (Meteorología) es responsable de:

   1. Desplegar el service bus (RabbitMQ) y coordinar con todos los grupos el esquema de
      colas/tópicos a utilizar.

   2. Desplegar el API Gateway / Reverse Proxy que actuará como punto de entrada único
      para los clientes web.

   3. Montar la infraestructura de la base de datos principal (la de Torre de Control), aunque el
      diseño del esquema y el acceso a los datos siguen siendo responsabilidad del Grupo 1.

   4. Documentar el proceso de despliegue para que el resto de los grupos pueda integrarse sin
      fricción.

Cada uno de los demás grupos es responsable de desplegar y mantener su propia base de
datos y su propio servicio (contenedorizado con Docker).
## Requerimientos no funcionales
- Autenticación entre servicios (JWT): Seguridad y Usuarios (Grupo 7) es el único emisor de
       tokens JWT del sistema, y contempla dos flujos distintos:
- Login humano (pasajeros, operadores, administradores): credenciales de usuario →
              token con rol asociado. Aplica a las interacciones de Check-in y del Panel de
              Administración con sus propios usuarios.
- Service-to-service (client credentials): cada servicio que expone endpoints
              protegidos define su propia API key (client id/secret) y la registra en Seguridad,
              entregándosela también a los grupos que necesiten consumirlo. El grupo
              consumidor almacena esa key en su appsettings.json y la usa para solicitar un token
              de servicio a Seguridad antes de invocar el endpoint protegido, adjuntándolo
              como Authorization: Bearer <token>.
- En ambos casos, cada servicio valida localmente la firma y el audience del token
              recibido — no necesita consultar a Seguridad en cada request.
- Manejo de fallas entre servicios:
- Toda llamada REST entre grupos debe definir un timeout máximo (recomendado: 5
              segundos).
- Ante un fallo o timeout, el servicio origen debe reintentar de forma acotada
              (recomendado: hasta 3 reintentos con backoff) antes de darse por vencido.
- Si el servicio destino sigue sin responder, el origen debe registrar el error en Logs
              (Grupo 8) y devolver una respuesta controlada a quien lo llamó — nunca dejar la
              solicitud colgada.
- Cada grupo puede sumar mecanismos adicionales (ej. circuit breaker) si lo
                considera necesario, documentándolo en su entrega.
- Idempotencia de eventos del bus: todo evento publicado a RabbitMQ debe incluir un
         timestamp de emisión (ver envelope en sección 8). Cada consumidor debe comparar ese
         timestamp contra el último evento procesado del mismo tipo y origen, para descartar
         duplicados o eventos que lleguen fuera de orden.
- Registro de logs centralizado (Grupo 8), identificando el componente que originó cada
         evento.
- Todos los servicios deben estar contenedorizados (Docker).
- Los servicios deben ser escalables horizontalmente (al menos de forma
         conceptual/documentada).
- Las interconexiones entre grupos deben respetar los contratos de API/mensajes de la
         sección 8, documentados y acordados entre las partes.
## Contratos técnicos
Estos contratos son la base común que todos los grupos deben respetar. Cada grupo puede
agregar campos adicionales al payload interno de sus eventos o al body de sus endpoints según
lo necesite, siempre manteniendo la estructura base.
### Token JWT (emitido por Seguridad — Grupo 7)
{
    "sub": "grupo-vuelos-service",
    "aud": "torre-control-api",
    "role": "service",
    "iat": 1755870600,
    "exp": 1755874200
}
### Envelope estándar de eventos del bus (RabbitMQ)
{
    "eventId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "tipo": "AutorizacionOtorgada",
    "origen": "TorreControl",
    "timestamp": "2026-08-22T14:30:00Z",
    "payload": { }
}
- eventId: identificador único del evento (GUID).
- tipo: nombre del evento (ej. AutorizacionOtorgada, EstadoVueloCambiado,
         PistaLiberada).
- origen: nombre del grupo/servicio que lo publica.
- timestamp: momento de emisión, usado por los consumidores para idempotencia
         (sección 7).
- payload: datos específicos del evento, definidos por el grupo emisor y documentados
         junto a su módulo.
### Ejemplo de contrato REST — Solicitud de autorización (Vuelos → Torre de Control)
### Request
POST /api/autorizaciones
Authorization: Bearer <token>
Content-Type: application/json

{
    "vueloId": "AR1234-20260822",
    "pistaSolicitada": "09L",
    "tipoOperacion": "Despegue"
}
### Response
{
    "autorizado": true,
    "pista": "09L",
    "motivoDenegacion": null,
    "timestamp": "2026-08-22T14:30:05Z"
}

Este es un ejemplo de referencia para fijar el criterio (autenticación, formato de fecha, nombres
en camelCase). Cada grupo debe formalizar y documentar el contrato específico de cada
endpoint que exponga, respetando esta base.
## Entregables
### Documentación
  ▪   Aspectos Descriptivos de la Solución Tecnológica. Además, para cada componente
      detallar:
- Lenguaje de programación / tecnologías utilizadas
- Dependencias / versiones de librerías utilizadas
- Cualquier otro aspecto para considerar (por ej.: qué sistemas operativos soporta,
             etc.)
  ▪   Descripción reducida del sistema (de qué trata, cuál es su finalidad)
  ▪   Limitaciones de la solución (definición del alcance)
  ▪   Requerimientos: (listar los requerimientos desarrollados)
- Funcionales
- No funcionales
- De negocio
  ▪   Contratos de interconexión: qué expone, qué consume, de quién y con qué protocolo.
  ▪   Al menos, el diseño de tres casos de prueba de los escenarios más importantes.
  ▪   Manual de usuario del sistema
  ▪   Manual de instalación/despliegue de los componentes
### Código fuente
- Repositorio del módulo comprimido en ZIP.
## Criterio de evaluación
  ✓ Cumplimiento de los requerimientos funcionales del módulo propio.

  ✓ Correcta implementación de la(s) interconexión(es) obligatoria(s) con otros grupos.

  ✓ Calidad y completitud de la documentación.

  ✓ Uso apropiado del stack tecnológico (obligatorio + justificación de alternativas).

  ✓ Presentación/defensa oral.

