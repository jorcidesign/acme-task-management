-- ============================================================
-- ACME Task Management System — DDL & DML
-- ============================================================

-- 1. ENUM
CREATE TYPE task_status AS ENUM ('pending', 'in_progress', 'completed');

-- 2. TABLA: users
CREATE TABLE users (
    id            BIGSERIAL    PRIMARY KEY,
    email         VARCHAR(255) NOT NULL,
    full_name     VARCHAR(150) NOT NULL,
    password_hash VARCHAR(72)  NOT NULL,
    created_at    TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    updated_at    TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_users_email UNIQUE (email)
);

-- 3. TABLA: tasks (Con Soft Delete)
CREATE TABLE tasks (
    id          BIGSERIAL    PRIMARY KEY,
    user_id     BIGINT       NOT NULL,
    title       VARCHAR(255) NOT NULL,
    description TEXT,
    status      task_status  NOT NULL DEFAULT 'pending',
    due_date    TIMESTAMPTZ,
    is_deleted  BOOLEAN      NOT NULL DEFAULT FALSE,
    created_at  TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    updated_at  TIMESTAMPTZ  NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_tasks_user FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE RESTRICT
);

COMMENT ON TABLE  tasks            IS 'Tareas asignadas a los usuarios del sistema.';
COMMENT ON COLUMN tasks.is_deleted IS 'Bandera de borrado lógico (Soft Delete).';

-- 4. ÍNDICES OPTIMIZADOS (Ignoran las tareas borradas)
CREATE INDEX idx_tasks_user_status ON tasks (user_id, status)
    WHERE is_deleted = FALSE;

CREATE INDEX idx_tasks_user_due ON tasks (user_id, due_date)
    WHERE due_date IS NOT NULL AND is_deleted = FALSE;

-- 5. FUNCIÓN Y TRIGGERS: updated_at automático
CREATE OR REPLACE FUNCTION set_updated_at()
RETURNS TRIGGER LANGUAGE plpgsql AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$;

CREATE TRIGGER trg_users_updated_at
    BEFORE UPDATE ON users
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

CREATE TRIGGER trg_tasks_updated_at
    BEFORE UPDATE ON tasks
    FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- ============================================================
-- SEED DATA — 10 usuarios (refleja el caso real de ACME)
-- Contraseñas: passwordN para el usuario N (ej. password1, password2...)
-- Usuarios testeables con la app: alice (password1) y bob (password2)
-- ============================================================

INSERT INTO users (email, full_name, password_hash) VALUES
    ('alice@acme.com',   'Alice García',    '$2a$12$pqBPOFxtU0OpKPxHAq0TP.nb8rWtHT0j4r8kR37bjpdJHxS26YNCq'),
    ('bob@acme.com',     'Bob Ramírez',     '$2a$12$5wwSEUPn2sjugI7uC5nIPekcGXEYrdFoNQ7zqqRaY5KMhVWawzKfm'),
    ('carol@acme.com',   'Carol Mendoza',   '$2a$12$3ANLlOHoILkqOnOFQH5LwOwV2suAbxjdDEL2OdxAOk8.26ArL8JKu'),
    ('david@acme.com',   'David Torres',    '$2a$12$10EwM/8wOamA1ZRb.P0Whetl23HWFfy4Fx.YQ86T5flPYhckoNxGW'),
    ('elena@acme.com',   'Elena Vargas',    '$2a$12$68uoO4kRzosVyGoGvzcO/OXWkARjTdww6YZr/6tGwcTNL0Mu8QTuC'),
    ('frank@acme.com',   'Frank Quispe',    '$2a$12$O6w7OsdN77PUuFxaJV53Ze8u8dXVbW4mneKZts6qz6ivGCtgWCFpi'),
    ('grace@acme.com',   'Grace Huamán',    '$2a$12$WLlnBEpE7ASARoZgwYpoX.Xfaz2Nb7C8YNONjaSSk3ZiCoVL5S1lG'),
    ('henry@acme.com',   'Henry Castillo',  '$2a$12$Tf2ivBPQrTVRHckYM.WeHu/40oRPhFJ76oCqjVKetPYpeDTLflqJG'),
    ('isabel@acme.com',  'Isabel Flores',   '$2a$12$/83I6RJpf6oFNBsB2VW4au3p1Y4qvYdMN7bibs9o9KR4iqHhK4BUK'),
    ('jorge@acme.com',   'Jorge Salinas',   '$2a$12$tl4cm7AWTb3VFP1IWO87RevQOkwWGyxWntYXrKNaMkKilVu/fjzU.');

INSERT INTO tasks (user_id, title, description, status, due_date) VALUES
    -- Alice (user_id = 1) — password1
    (1, 'Migrar base de datos a PostgreSQL',  'Exportar el Excel actual y definir el esquema relacional.', 'in_progress', NOW() + INTERVAL '3 days'),
    (1, 'Revisar diseño de la API REST',       NULL,                                                          'pending',     NOW() + INTERVAL '7 days'),
    (1, 'Preparar informe mensual',            'Consolidar métricas del mes en el dashboard.',                'pending',     NOW() + INTERVAL '5 days'),

    -- Bob (user_id = 2) — password2
    (2, 'Configurar entorno Angular',          'Instalar dependencias y configurar ambiente local.',           'completed',   NOW() - INTERVAL '1 day'),
    (2, 'Documentar endpoints con Swagger',    'Anotar los controllers con atributos XML.',                   'pending',     NULL),
    (2, 'Revisar pull requests del sprint',    'Hacer code review de los PRs pendientes en GitHub.',          'in_progress', NOW() + INTERVAL '2 days'),

    -- Carol (user_id = 3) — password3
    (3, 'Actualizar manual de onboarding',     'Incorporar cambios del nuevo sistema de tareas.',             'pending',     NOW() + INTERVAL '10 days'),
    (3, 'Coordinar reunión de kickoff',        'Agendar con el equipo para el lunes.',                        'completed',   NOW() - INTERVAL '3 days'),

    -- David (user_id = 4) — password4
    (4, 'Configurar pipeline de CI/CD',        'Automatizar build y deploy con GitHub Actions.',              'in_progress', NOW() + INTERVAL '4 days'),
    (4, 'Levantar entorno de staging',         'Replicar prod en servidor de pruebas.',                       'pending',     NOW() + INTERVAL '6 days'),

    -- Elena (user_id = 5) — password5
    (5, 'Diseñar mockups de pantalla login',   'Figma: wireframes y propuesta visual.',                       'completed',   NOW() - INTERVAL '2 days'),
    (5, 'Validar flujo de usuario con QA',     'Sesión de pruebas funcionales con el equipo.',                'pending',     NOW() + INTERVAL '8 days'),

    -- Frank (user_id = 6) — password6
    (6, 'Revisar contratos de proveedores',    'Verificar vencimientos y renovaciones pendientes.',           'pending',     NOW() + INTERVAL '14 days'),
    (6, 'Archivar documentación Q1',           'Mover reportes finales al repositorio compartido.',           'completed',   NOW() - INTERVAL '5 days'),

    -- Grace (user_id = 7) — password7
    (7, 'Capacitar equipo en nuevo sistema',   'Sesión de 1h para presentar la herramienta.',                 'pending',     NOW() + INTERVAL '9 days'),
    (7, 'Registrar incidencias de soporte',    'Cargar tickets del mes anterior al sistema.',                 'in_progress', NOW() + INTERVAL '1 day'),

    -- Henry (user_id = 8) — password8
    (8, 'Auditar permisos de usuarios',        'Verificar roles y accesos en todos los sistemas.',            'pending',     NOW() + INTERVAL '12 days'),
    (8, 'Actualizar dependencias del backend', 'Revisar vulnerabilidades con dotnet list package.',           'in_progress', NOW() + INTERVAL '3 days'),

    -- Isabel (user_id = 9) — password9
    (9, 'Analizar feedback de usuarios',       'Revisar respuestas de la encuesta interna de satisfacción.', 'pending',     NOW() + INTERVAL '6 days'),
    (9, 'Preparar presentación de resultados', 'Slides para la reunión de dirección del viernes.',            'in_progress', NOW() + INTERVAL '2 days'),

    -- Jorge (user_id = 10) — password10
    (10, 'Optimizar consultas lentas en BD',   'Analizar explain plan de las queries críticas.',              'pending',     NOW() + INTERVAL '5 days'),
    (10, 'Escribir tests de integración',      'Cubrir los endpoints principales con pruebas automatizadas.','pending',     NOW() + INTERVAL '11 days');
