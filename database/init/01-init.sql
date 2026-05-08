-- ============================================================
-- ACME Task Management System — DDL & DML
-- ============================================================

CREATE TYPE task_status AS ENUM ('pending', 'in_progress', 'completed');

CREATE TABLE users (
    id BIGSERIAL PRIMARY KEY,
    email VARCHAR(255) NOT NULL,
    full_name VARCHAR(150) NOT NULL,
    password_hash VARCHAR(72) NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT uq_users_email UNIQUE (email)
);

-- 3. TABLA: tasks (Con Soft Delete)
CREATE TABLE tasks (
    id BIGSERIAL PRIMARY KEY,
    user_id BIGINT NOT NULL,
    title VARCHAR(255) NOT NULL,
    description TEXT,
    status task_status NOT NULL DEFAULT 'pending',
    due_date TIMESTAMPTZ,
    is_deleted BOOLEAN NOT NULL DEFAULT FALSE,
    created_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    updated_at TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    CONSTRAINT fk_tasks_user FOREIGN KEY (user_id) REFERENCES users (id) ON DELETE RESTRICT
);

COMMENT ON TABLE tasks IS 'Tareas asignadas a los usuarios del sistema.';

COMMENT ON COLUMN tasks.is_deleted IS 'Bandera de borrado lógico (Soft Delete).';

-- 4. ÍNDICES OPTIMIZADOS (Ignoran las tareas borradas)
CREATE INDEX idx_tasks_user_status ON tasks (user_id, status)
WHERE
    is_deleted = FALSE;

CREATE INDEX idx_tasks_user_due ON tasks (user_id, due_date)
WHERE
    due_date IS NOT NULL
    AND is_deleted = FALSE;

CREATE OR REPLACE FUNCTION set_updated_at() RETURNS TRIGGER LANGUAGE plpgsql AS $$
BEGIN
    NEW.updated_at = NOW();
    RETURN NEW;
END;
$$;

CREATE TRIGGER trg_users_updated_at BEFORE UPDATE ON users FOR EACH ROW EXECUTE FUNCTION set_updated_at();

CREATE TRIGGER trg_tasks_updated_at BEFORE UPDATE ON tasks FOR EACH ROW EXECUTE FUNCTION set_updated_at();

-- SEED DATA
INSERT INTO
    users (
        email,
        full_name,
        password_hash
    )
VALUES (
        'alice@acme.com',
        'Alice García',
        '$2a$12$pqBPOFxtU0OpKPxHAq0TP.nb8rWtHT0j4r8kR37bjpdJHxS26YNCq'
    ),
    (
        'bob@acme.com',
        'Bob Ramírez',
        '$2a$12$5wwSEUPn2sjugI7uC5nIPekcGXEYrdFoNQ7zqqRaY5KMhVWawzKfm'
    );

INSERT INTO
    tasks (
        user_id,
        title,
        description,
        status,
        due_date
    )
VALUES (
        1,
        'Migrar base de datos a PostgreSQL',
        'Exportar el Excel actual...',
        'in_progress',
        NOW() + INTERVAL '3 days'
    ),
    (
        1,
        'Revisar diseño de la API REST',
        NULL,
        'pending',
        NOW() + INTERVAL '7 days'
    ),
    (
        2,
        'Configurar entorno Angular',
        'Instalar dependencias...',
        'completed',
        NOW() - INTERVAL '1 day'
    ),
    (
        2,
        'Documentar endpoints con Swagger',
        'Anotar los controllers...',
        'pending',
        NULL
    );