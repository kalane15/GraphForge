# GraphForge API CRUD

## Entities

- User
- Project
- Graph
- Schema
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

Project is a container for graphs and schemas.

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

# Schema

Schema describes available node types.

Example:

    schemaTypeName: DialogueNode
    content:
      fields:
      - name: text
        type: string
      - name: speaker
        type: string

---

    POST /projects/{projectId}/schemas

Create a new schema.

    GET /projects/{projectId}/schemas

Get all schemas in a project.

    GET /schemas/{schemaId}

Get schema.

    PUT /schemas/{schemaId}

Update schema.

    DELETE /schemas/{schemaId}

Delete schema.
