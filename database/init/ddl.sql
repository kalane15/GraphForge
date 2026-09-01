CREATE TABLE IF NOT EXISTS sessions
(
    id            UUID PRIMARY KEY,
    refresh_token_hash VARCHAR(255) NOT NULL,
    expires_at    TIMESTAMPTZ NOT NULL,
	revoked_at    TIMESTAMPTZ DEFAULT NULL
);

CREATE TABLE IF NOT EXISTS users
(
    id            UUID PRIMARY KEY,
    login         VARCHAR(255) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    created_at    TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
	session_id     UUID,

	CONSTRAINT fk_sessions_owner
        FOREIGN KEY (session_id)
        REFERENCES sessions(id)
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

CREATE TABLE IF NOT EXISTS graph_documents
(
    id         UUID PRIMARY KEY,
    project_id UUID NOT NULL,
    name       VARCHAR(255) NOT NULL,
    content    JSONB NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_graph_documents_project
        FOREIGN KEY (project_id)
        REFERENCES projects(id)
        ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS node_schemas
(
    id         UUID PRIMARY KEY,
    project_id UUID NOT NULL,
    name       VARCHAR(255) NOT NULL,
    schema     JSONB NOT NULL,
    created_at TIMESTAMPTZ NOT NULL DEFAULT CURRENT_TIMESTAMP,

    CONSTRAINT fk_node_schemas_project
        FOREIGN KEY (project_id)
        REFERENCES projects(id)
        ON DELETE CASCADE
);