@startuml
!theme plain

hide methods
hide stereotypes

entity "users" as users {
    * id : uuid <<PK>>
    --
    login : varchar
}

entity "projects" as projects {
    * id : uuid <<PK>>
    --
    owner_id : uuid <<FK>>
    name : varchar
    description : text
    created_at : timestamp
    updated_at : timestamp
}

entity "graph_documents" as graphs {
    * id : uuid <<PK>>
    --
    project_id : uuid <<FK>>
    name : varchar
    content : jsonb
    created_at : timestamp
    updated_at : timestamp
}

users ||--o{ projects : owns

projects ||--o{ graphs : contains



@enduml