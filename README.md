# ACME Task Management

Sistema web de gestión de tareas para equipos internos. Reemplaza el Excel compartido con una app fullstack con autenticación y persistencia.

---

## Stack

| Capa | Tecnología |
|---|---|
| Frontend | Angular 17+ (Standalone, Signals) |
| Backend | .NET 8 / ASP.NET Core Web API |
| Base de datos | PostgreSQL 15 |
| Infraestructura | Docker + Docker Compose + Nginx |

---

## Cómo correr el proyecto

**Requisitos:** Docker y Docker Compose instalados. Puertos `4200`, `8080` y `5433` disponibles.

```bash
git clone https://github.com/jorcidesign/acme-task-management.git
cd acme-task-management
docker compose up -d --build
```

- Frontend → http://localhost:4200  
- API → http://localhost:8080/api

**Credenciales de prueba:**

| Email | Password |
|---|---|
| alice@acme.com | password1 |
| bob@acme.com | password2 |

---

## Decisiones técnicas

### Base de datos
- ENUMs nativos de PostgreSQL para estados de tarea — validación a nivel de motor, no de aplicación.
- `TIMESTAMPTZ` para todas las fechas (UTC, internacionalizable).
- Trigger SQL para `updated_at` automático — sin depender de lógica de app.
- Índice compuesto `(user_id, status)` para las consultas más frecuentes.
- `ON DELETE RESTRICT` para preservar historial de tareas ante eliminación de usuarios.

### Backend
- Arquitectura N-Tier con separación clara: Controllers → Services → Repositories.
- DTOs como `record` de C# — inmutables, desacoplados de las entidades de EF Core.
- JWT con `UserId` extraído desde claims — aislamiento de datos por usuario en todas las queries.
- Contraseñas hasheadas con BCrypt (`BCrypt.Net-Next`), nunca en texto plano.

### Frontend
- Standalone Components sin NgModules — menos boilerplate, mejor tree-shaking.
- Estado reactivo con Signals (`signal`, `computed`, `effect`) — más predecible que RxJS para estado local.
- `HttpInterceptorFn` para inyección automática del JWT en cada request.
- Control flow moderno (`@if`, `@for`) en templates.
