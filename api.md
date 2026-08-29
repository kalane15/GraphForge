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

---

## CRUD

### Get current user

    GET /users/me

Get authenticated user's information.

---

### Update profile

    PUT /users/me

Update user profile information.

---

### Delete account

    DELETE /users/me

Delete user account.

---

# Project

Project is a container for graphs and node schemas.

---

## Create

    POST /projects

Create a new project.

---

## Read

### Get user's projects

    GET /projects

Get all projects owned by current user.

---

### Get project

    GET /projects/{projectId}

Get project information.

---

## Update

    PUT /projects/{projectId}

Update project information.

---

## Delete

    DELETE /projects/{projectId}

Delete project.

---

# Graph

Graph represents a graph structure containing nodes and edges.

---

## Create

    POST /projects/{projectId}/graphs

Create a new graph inside a project.

---

## Read

### Get project graphs

    GET /projects/{projectId}/graphs

Get all graphs inside a project.

---

### Get graph

    GET /graphs/{graphId}

Get graph data.

---

## Update

    PUT /graphs/{graphId}

Save graph changes.

---

## Delete

    DELETE /graphs/{graphId}

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

## Create

    POST /projects/{projectId}/schemas

Create a new node schema.

---

## Read

### Get project schemas

    GET /projects/{projectId}/schemas

Get all node schemas in a project.

---

### Get schema

    GET /schemas/{schemaId}

Get node schema.

---

## Update

    PUT /schemas/{schemaId}

Update node schema.

---

## Delete

    DELETE /schemas/{schemaId}

Delete node schema.

---

# GraphVersion

Stores graph history.

---

## Create

    POST /graphs/{graphId}/versions

Create a new graph version.

---

## Read

### Get graph versions

    GET /graphs/{graphId}/versions

Get all graph versions.

---

### Get version

    GET /graphs/{graphId}/versions/{version}

Get specific graph version.

---

## Restore

    POST /graphs/{graphId}/versions/{version}/restore

Restore graph from selected version.

---

## Delete

    DELETE /graphs/{graphId}/versions/{version}

Delete graph version.

---

# Export

Export graph data for runtime usage.

---

## Create

    POST /graphs/{graphId}/export

Generate graph export file.

---

## Read

### Get export information

    GET /exports/{exportId}

Get export metadata.

---

### Download export

    GET /exports/{exportId}/download

Download exported graph file.

---

## Delete

    DELETE /exports/{exportId}

Delete export.

---

# Summary

| Entity | Create | Read | Update | Delete |
|---|---|---|---|---|
| User | OAuth | Yes | Yes | Yes |
| Project | Yes | Yes | Yes | Yes |
| Graph | Yes | Yes | Yes | Yes |
| NodeSchema | Yes | Yes | Yes | Yes |
| GraphVersion | Yes | Yes | No | Yes |
| Export | Yes | Yes | No | Yes |