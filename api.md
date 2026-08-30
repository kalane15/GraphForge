# GraphForge API CRUD

## Entities

- User
- Project
- Graph
- NodeSchema
- GraphVersion
- Export

---

# User

User is created through GitHub OAuth.

## Authentication

    POST /auth/github

Authenticate user via GitHub OAuth.

    GET /users/{id}

    PUT /users/{id}

    DELETE /users/{id}

---

# Project

Project is a container for graphs and node schemas.

---

    POST /projects

Create a new project.

    GET /projects

Get all projects owned by current user.

    GET /projects/{projectId}

Get project information.

    PUT /projects/{projectId}

Update project information.

    DELETE /projects/{projectId}

Delete project.

---

# Graph

Graph represents a graph structure containing nodes and edges.

    POST /projects/{projectId}/graphs

Create a new graph inside a project.

    GET /projects/{projectId}/graphs

Get all graphs inside a project.

    GET /projects/{projectId}/graphs/{graphId}

Get graph data.

    POST /projects/{projectId}/graphs/{graphId}/import/json

Return json file with graph

    POST /projects/{projectId}/import/json

Import graph from file to project. Returns id of new graph


    GET /projects/{projectId}/export/json

Export graph from project to file

    PUT /projects/{projectId}/graphs/{graphId}

Save graph changes.


    DELETE /projects/{projectId}/graphs/{graphId}

Delete graph.

---

# NodeSchema

NodeSchema describes available node types.

Example:

    DialogueNode

    Fields:
    - text: string
    - speaker: string

---

    POST /projects/{projectId}/schemas

Create a new node schema.

    GET /projects/{projectId}/schemas

Get all node schemas in a project.

    GET /schemas/{schemaId}

Get node schema.

    PUT /schemas/{schemaId}

Update node schema.

    DELETE /schemas/{schemaId}

Delete node schema.
