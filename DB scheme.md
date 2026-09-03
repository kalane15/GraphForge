@startuml
!theme plain

hide methods
hide stereotypes

entity "sessions" as sessions{
    * id : uuid <<PK>>
    --
    user_id : uuid <<FK>>
    expires_at : timestamptz
	revoked_at : timestamptz
	refresh_token_hash : varchar
}

entity "users" as users {
    * id : uuid <<PK>>
    --
    login : varchar
	password_hash: varchar
    created_at : timestamptz
}

entity "projects" as projects {
    * id : uuid <<PK>>
    --
    owner_id : uuid <<FK>>
    name : varchar
    description : text
    created_at : timestamptz
    updated_at : timestamptz
}

entity "graphs" as graphs {
    * id : uuid <<PK>>
    --
    project_id : uuid <<FK>>
    name : varchar
    content : jsonb
    created_at : timestamptz
    updated_at : timestamptz
}

entity "node_schemas" as schemas {
    * id : uuid <<PK>>
    --
    project_id : uuid <<FK>>
    name : varchar
    schema : jsonb
    created_at : timestamptz
}

users ||--o{ projects : owns

users ||--o{ sessions : has

projects ||--o{ graphs : contains

projects ||--o{ schemas : defines


@enduml
