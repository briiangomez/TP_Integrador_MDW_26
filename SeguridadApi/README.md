# SeguridadApi — Grupo 7 (Seguridad y Usuarios)

Servidor de Autorización centralizado del sistema aeroportuario. Es el **único emisor de tokens JWT**.

- Los **usuarios humanos** obtienen un token con `POST /api/auth/login`.
- Los **microservicios (M2M)** obtienen un token con `POST /api/auth/token` (Client Credentials).
- Cada servicio **valida el token localmente** (firma + audience + expiración). No hay que llamar a Seguridad en cada request.

> Swagger (solo en Development): `http://localhost:5099/swagger`

---

## 1. Contrato del token

```json
{
  "sub": "grupo-vuelos-service",
  "aud": "torre-control-api",
  "role": "service",
  "iat": 1755870600,
  "exp": 1755874200
}
```

| Claim  | Significado |
|--------|-------------|
| `sub`  | Quién pide el token: `clientId` (servicios) o `username` (humanos). |
| `aud`  | API(s) para las que sirve el token. String si es una sola, array si son varias (RFC 7519 §4.1.3). |
| `role` | Rol del sujeto (`service`, `administrador`, `operador-tierra`, `controlador-aereo`). |
| `iat` / `exp` | Emisión y expiración (Unix time). |

- Firma: **HS256**.
- Claims planos (sin URIs de Microsoft). **No incluye `iss`.**
- **Usuarios:** un solo login. El token incluye todos los audiences habilitados para su rol.
- **Servicios:** un token por audience. Si un servicio llama a dos APIs, pide dos tokens.

Token de usuario (varios audiences):

```json
{
  "sub": "operador1",
  "aud": ["checkin-api", "torre-control-api"],
  "role": "administrador",
  "iat": 1755870600,
  "exp": 1755874200
}
```

---

## 2. Cómo consumir el servicio

### 2.1 Login de usuario humano

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "operador1",
  "password": "********"
}
```

El login no recibe `audience`: el token incluye siempre **todos** los audiences habilitados para el rol del usuario.

### 2.2 Token de servicio (Client Credentials / M2M)

```http
POST /api/auth/token
Content-Type: application/json

{
  "clientId": "grupo-vuelos-service",
  "clientSecret": "********",
  "audience": "torre-control-api"
}
```

### 2.3 Respuestas (ambos endpoints)

| Código | Cuándo | Body |
|--------|--------|------|
| `200`  | Credenciales válidas y el rol tiene acceso al `audience` | `{ "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJncnVwby12dWVsb3Mtc2VydmljZSIsInJvbGUiOiJzZXJ2aWNlX3Z1ZWxvcyIsImF1ZCI6InRvcnJlLWNvbnRyb2wtYXBpIiwibmJmIjoxNzkxMjk0ODE3LCJleHAiOjE3OTEzMDIwMTcsImlhdCI6MTc5MTI5NDgxN30.8VUKnBBn2f-p74VqYEvVWaPXVrNkIKBzIrWeIetZwXg" }` |
| `400`  | Falta un campo obligatorio, viene vacío o el JSON es inválido. En `/token`: el `audience` no existe o no está activo | ProblemDetails con `errors: string[]` |
| `401`  | Usuario/cliente inexistente, inactivo o credencial incorrecta | — |
| `403`  | `/token`: el rol del cliente no tiene permiso sobre el `audience`. `/login`: el rol no tiene ningún audience habilitado | ProblemDetails con `detail` |
| `500`  | Error inesperado (ej. base de datos caída) | ProblemDetails |

### 2.4 Usar el token

```http
POST /api/autorizaciones
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJncnVwby12dWVsb3Mtc2VydmljZSIsInJvbGUiOiJzZXJ2aWNlX3Z1ZWxvcyIsImF1ZCI6InRvcnJlLWNvbnRyb2wtYXBpIiwibmJmIjoxNzkxMjk0ODE3LCJleHAiOjE3OTEzMDIwMTcsImlhdCI6MTc5MTI5NDgxN30.8VUKnBBn2f-p74VqYEvVWaPXVrNkIKBzIrWeIetZwXg
```

### 2.5 Ejemplo con curl

```bash
curl -X POST http://localhost:5099/api/auth/token \
  -H "Content-Type: application/json" \
  -d '{"clientId":"grupo-vuelos-service","clientSecret":"secreto","audience":"torre-control-api"}'
```

---

## 3. Guía para servicios CONSUMIDORES (quien pide el token)

1. Pedile al grupo dueño de la API (proveedor) tu `clientId` / `clientSecret`. Ese grupo la registra en Seguridad.
2. Guardala en tu `appsettings.json`:

```json
"Seguridad": {
  "BaseUrl": "http://seguridad-api:8080",
  "ClientId": "grupo-vuelos-service",
  "ClientSecret": "********"
}
```

3. **Reutilizá el token hasta que venza.** No pidas uno por request.
4. Respetá los RNF del TP: **timeout 5 s** y **hasta 3 reintentos con backoff**.
5. Si el proveedor responde `401`, descartá el token cacheado y pedí uno nuevo (una sola vez).

Uso:

```csharp
var token = await tokenProvider.GetTokenAsync("torre-control-api");
request.Headers.Authorization = new("Bearer", token);
```

---

## 4. Guía para servicios PROVEEDORES (quien valida el token)

### 4.1 Qué hay que validar

| Validación | Si falla |
|------------|----------|
| Header `Authorization: Bearer <token>` presente | 401 |
| Firma HS256 válida | 401 |
| `exp` vigente | 401 |
| `aud` contiene **tu** audience (ej. `torre-control-api`). Puede venir como string o como array | 401 |
| `role` permitido para el endpoint (si aplica) | 403 |

### 4.2 Configuración (.NET 8/9)

Paquete:

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

`appsettings.json`:

```json
"Seguridad": {
  "JwtSecret": "<secret compartido por el Grupo 7>",
  "Audience": "torre-control-api"
}
```

`Program.cs`:

```csharp
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Mantener los claims con sus nombres cortos (sub, aud, role)
        options.MapInboundClaims = false;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Seguridad:JwtSecret"]!)),

            ValidateIssuer = false,                 // el token no trae "iss"

            ValidateAudience = true,
            ValidAudience = builder.Configuration["Seguridad:Audience"],

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30),

            NameClaimType = "sub",
            RoleClaimType = "role"                  // habilita RequireRole / [Authorize(Roles=...)]
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SoloServicios", p => p.RequireRole("service"));
    options.AddPolicy("SoloAdmin", p => p.RequireRole("administrador"));
});

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Cualquier token válido para mi audience
app.MapPost("/api/autorizaciones", (...) => { ... })
   .RequireAuthorization("SoloServicios");

// Además exige rol administrador
app.MapPut("/api/reglas", (...) => { ... })
   .RequireAuthorization("SoloAdmin");
```

> `ValidAudience` ya acepta `aud` como string o como array. Valida que tu audience esté entre los valores.

Leer quién llama:

```csharp
app.MapGet("/api/quien-soy", (ClaimsPrincipal user) => new
{
    sub = user.FindFirst("sub")?.Value,
    role = user.FindFirst("role")?.Value
}).RequireAuthorization();
```
---
