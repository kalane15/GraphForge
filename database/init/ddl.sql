CREATE TABLE IF NOT EXISTS users
(
    id            UUID PRIMARY KEY,
    login         VARCHAR(255) NOT NULL UNIQUE,
    password_hash VARCHAR(255) NOT NULL,
    created_at    TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE IF NOT EXISTS sessions
(
    id                 UUID PRIMARY KEY,
    user_id            UUID NOT NULL,
    refresh_token_hash VARCHAR(255) NOT NULL,
    expires_at         TIMESTAMPTZ NOT NULL,
    revoked_at         TIMESTAMPTZ DEFAULT NULL,

    CONSTRAINT fk_sessions_user
        FOREIGN KEY (user_id)
        REFERENCES users(id)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS projects
(
    id          UUID PRIMARY KEY,
    owner_id    UUID NOT NULL,
    name        VARCHAR(255) NOT NULL,
    description TEXT,
    created_at  TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at  TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_projects_owner
        FOREIGN KEY (owner_id)
        REFERENCES users(id)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS graphs
(
    id         UUID PRIMARY KEY,
    project_id UUID NOT NULL,
    name       VARCHAR(255) NOT NULL,
    content    JSONB NOT NULL DEFAULT '{"nodes":[],"edges":[]}'::jsonb,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_graphs_project
        FOREIGN KEY (project_id)
        REFERENCES projects(id)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS schemas
(
    id         UUID PRIMARY KEY,
    project_id UUID NOT NULL,
    schema_type_name VARCHAR(255) NOT NULL,

    CONSTRAINT fk_schemas_project
        FOREIGN KEY (project_id)
        REFERENCES projects(id)
        ON DELETE CASCADE,

    CONSTRAINT uq_schemas_project_id
        UNIQUE (project_id, id),

    CONSTRAINT uq_schemas_project_schema_type_name
        UNIQUE (project_id, schema_type_name)
);

CREATE TABLE IF NOT EXISTS schema_fields
(
    id         UUID PRIMARY KEY,
    schema_id  UUID NOT NULL,
	name      VARCHAR(255) NOT NULL,
	type      VARCHAR(255) NOT NULL DEFAULT 'string',

    CONSTRAINT fk_fields_schemas
        FOREIGN KEY (schema_id)
        REFERENCES schemas(id)
        ON DELETE CASCADE
);
