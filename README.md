# 🚀 ACME Task Management System

Sistema integral de gestión de tareas construido como solución al control, seguimiento y concurrencia de las tareas internas de la empresa ACME.

Esta aplicación Fullstack está diseñada con un fuerte enfoque en **escalabilidad, código limpio, arquitectura mantenible y experiencia de usuario (UI/UX)**, utilizando tecnologías modernas del ecosistema .NET y Angular.

---

# 🛠️ Stack Tecnológico

## Frontend
- Angular 17+
- Standalone Components
- Angular Signals
- Angular Control Flow (`@if`, `@for`)
- TypeScript
- Tailwind CSS

## Backend
- .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- Npgsql

## Base de Datos
- PostgreSQL 15+

## Infraestructura
- Docker
- Docker Compose
- Nginx
- Multi-stage Builds

---

# 🏗️ Arquitectura y Decisiones Técnicas

## Backend (.NET 8 API)

Se implementó una arquitectura en capas (**N-Tier Architecture**) orientada al dominio para garantizar separación de responsabilidades, mantenibilidad y escalabilidad.

### Principales decisiones técnicas

#### DTOs Inmutables
Uso estricto de DTOs mediante `record` de C# para:
- evitar exponer entidades internas,
- desacoplar persistencia de transporte,
- mejorar seguridad y mantenibilidad.

#### Autenticación y Seguridad
- Implementación de autenticación JWT.
- Middleware de autorización centralizado.
- Extracción del `UserId` desde los claims del token.
- Aislamiento de datos por usuario.

#### Hash de Contraseñas
Las contraseñas jamás se almacenan en texto plano.

Se utiliza:

- `BCrypt.Net-Next`

para:
- hash seguro,
- validación robusta,
- protección ante filtraciones.

#### Entity Framework Core
Uso de:
- migraciones,
- relaciones tipadas,
- configuración Fluent API,
- soporte completo PostgreSQL mediante `Npgsql`.

---

## Frontend (Angular 17+)

### Arquitectura Moderna
Proyecto completamente construido sin `NgModules` utilizando:

- Standalone Components

Esto reduce:
- complejidad,
- boilerplate,
- acoplamiento.

### Gestión de Estado Reactiva
Uso moderno de:

- `signal`
- `computed`
- `effect`

en reemplazo de patrones tradicionales basados excesivamente en RxJS para estado local.

Beneficios:
- menor complejidad,
- mejor performance,
- reactividad más predecible,
- código más limpio.

### Nuevo Control Flow
Uso del nuevo template syntax de Angular:
- `@if`
- `@for`
- `@switch`

permitiendo:
- templates más legibles,
- menos directivas estructurales,
- mejor performance.

### UI / UX
Diseño inspirado en:
- Notion,
- macOS,
- interfaces minimalistas modernas.

Características:
- Tailwind CSS,
- tipografía limpia,
- espaciado consistente,
- colores neutros,
- microinteracciones suaves,
- diseño responsivo.

### Interceptores HTTP
Uso de:
- `HttpInterceptorFn`

para:
- inyectar automáticamente JWT,
- centralizar autenticación,
- evitar duplicación de lógica.

---

## Base de Datos (PostgreSQL)

El esquema fue diseñado priorizando:
- integridad,
- consistencia,
- rendimiento,
- concurrencia.

### ENUMs a Nivel de Base de Datos
Uso de `ENUM` nativo PostgreSQL para estados de tareas.

Ventajas:
- validación desde el motor,
- integridad estricta,
- prevención de estados inválidos.

### TIMESTAMPTZ
Uso de:
- `TIMESTAMPTZ`

para manejo correcto de fechas en UTC y compatibilidad internacional.

### Triggers de Auditoría
La actualización automática de:
- `updated_at`

se delega directamente a PostgreSQL mediante:
- triggers,
- funciones SQL.

Esto evita depender de lógica de aplicación.

### Índices Optimizados
Se implementaron índices compuestos estratégicos para:
- filtrado rápido,
- consultas frecuentes,
- escalabilidad futura.

Ejemplo:
```sql
(user_id, status)
```

### Integridad Referencial
Uso explícito de:
```sql
ON DELETE RESTRICT
```

para evitar:
- pérdida accidental de historial,
- inconsistencias relacionales.

---

# 📦 Estructura del Monorepo

```txt
acme-task-management/
│
├── apps/
│   ├── frontend/        # Angular App
│   └── backend/         # .NET 8 API
│
├── database/
│   ├── migrations/
│   ├── seeds/
│   └── scripts/
│
├── nginx/
│
├── docker-compose.yml
│
└── README.md
```

---

# ⚙️ Ejecución del Proyecto con Docker

Todo el proyecto se encuentra completamente contenerizado.

No es necesario instalar:
- Node.js
- .NET SDK
- PostgreSQL

localmente.

---

# ✅ Requisitos Previos

Instalar:

- Docker
- Docker Compose

Verificar disponibilidad de puertos:

- `4200`
- `8080`
- `5433`

---

# 🚀 Pasos para Ejecutar

## 1. Clonar el repositorio

```bash
git clone https://github.com/jorcidesign/acme-task-management.git
cd acme-task-management
```

---

## 2. Levantar toda la infraestructura

```bash
docker compose up -d --build
```

Este comando levanta automáticamente:

- PostgreSQL
- Backend .NET API
- Frontend Angular
- Nginx

---

## 3. Verificar contenedores

```bash
docker compose ps
```

Deberías visualizar:

- `acme-db`
- `acme-api`
- `acme-web`

en estado:

```txt
Up
```

---

# 🌐 Accesos

## Frontend
```txt
http://localhost:4200
```

## Backend API
```txt
http://localhost:8080/api
```

---

# 🧪 Credenciales de Prueba

La base de datos inicializa automáticamente datos semilla.

## Usuario 1

```txt
Email: alice@acme.com
Password: password1
```

## Usuario 2

```txt
Email: bob@acme.com
Password: password2
```

Ambos usuarios poseen:
- tareas precargadas,
- distintos estados,
- datos listos para pruebas funcionales.

---

# 🧠 Características Técnicas Destacadas

- Arquitectura N-Tier
- Monorepo Fullstack
- Angular Standalone
- Angular Signals
- JWT Authentication
- PostgreSQL ENUMs
- PostgreSQL Triggers
- Dockerized Infrastructure
- Multi-stage Builds
- Tailwind CSS
- Clean Architecture Principles
- DTO Isolation
- User Data Isolation
- Secure Password Hashing
- Optimized Relational Design

---

# 📋 Comandos Útiles

## Detener servicios

```bash
docker compose down
```

## Reconstruir imágenes

```bash
docker compose up -d --build
```

## Ver logs

```bash
docker compose logs -f
```

## Eliminar volúmenes y reiniciar completamente

```bash
docker compose down -v
```

---

# 🧪 Testing y Escalabilidad

La arquitectura fue diseñada considerando:
- separación clara de responsabilidades,
- facilidad de testing,
- escalabilidad horizontal futura,
- mantenibilidad a largo plazo.

La estructura permite incorporar fácilmente:
- CQRS,
- Redis,
- WebSockets,
- Microservicios,
- Kubernetes,
- CI/CD pipelines,
- observabilidad,
- testing automatizado.

---

# 📌 Consideraciones Finales

Este proyecto no solo busca resolver el problema funcional de gestión de tareas, sino también demostrar:
- buenas prácticas de ingeniería,
- diseño de software moderno,
- arquitectura escalable,
- experiencia de desarrollo profesional,
- enfoque real de producción.

---

# 👨‍💻 Autor

Desarrollado como prueba técnica Fullstack utilizando tecnologías modernas del ecosistema Angular + .NET.

---
