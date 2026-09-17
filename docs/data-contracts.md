# GraphForge Data Contracts

This document describes canonical JSON formats used by GraphForge frontend,
backend API, runtime, import/export, validation, and code generation.

All JSON property names use `camelCase`.

## Schema Contracts

Schemas describe available node types and their editable fields.

### Schemas List

Used by:

- `GET /api/projects/{projectId}/schemas`
- frontend schema editor
- graph editor schema lookup
- graph validation

```json
{
  "schemas": [
    {
      "id": "6f3a2f1a-9b6d-4c5e-9b1a-123456789abc",
      "schemaTypeName": "DialogueNode",
      "fields": [
        {
          "id": "a7e1c5c0-1111-4444-9999-abcdefabcdef",
          "name": "speaker",
          "type": "string"
        }
      ]
    }
  ]
}
```

### Schema

Used by:

- schema API responses
- frontend state
- graph validation

```json
{
  "id": "6f3a2f1a-9b6d-4c5e-9b1a-123456789abc",
  "schemaTypeName": "DialogueNode",
  "fields": [
    {
      "id": "a7e1c5c0-1111-4444-9999-abcdefabcdef",
      "name": "speaker",
      "type": "string"
    }
  ]
}
```

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| `id` | `uuid` | yes | Stable schema id. |
| `schemaTypeName` | `string` | yes | Schema type name referenced by graph nodes and used for generated runtime classes. |
| `fields` | `SchemaField[]` | yes | Ordered list of fields available on nodes of this schema. |

### Schema Field

```json
{
  "id": "a7e1c5c0-1111-4444-9999-abcdefabcdef",
  "name": "speaker",
  "type": "string"
}
```

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| `id` | `uuid` | yes | Stable field id. |
| `name` | `string` | yes | Field name used as key inside node `properties`. |
| `type` | `string` | yes | Field value type. |

Allowed field types:

```text
string
int
float
bool
```

### Schema Create Request

Used by:

- `POST /api/projects/{projectId}/schemas`
- creating an empty schema
- importing schemas with predefined fields

```json
{
  "schemaTypeName": "DialogueNode",
  "fields": [
    {
      "name": "speaker",
      "type": "string"
    }
  ]
}
```

`fields` may be an empty array:

```json
{
  "schemaTypeName": "DialogueNode",
  "fields": []
}
```

### Schema Update Request

Used by:

- `PUT /api/projects/{projectId}/schemas/{schemaId}`

```json
{
  "schemaTypeName": "DialogueNode",
  "fields": [
    {
      "id": "a7e1c5c0-1111-4444-9999-abcdefabcdef",
      "name": "speaker",
      "type": "string"
    }
  ]
}
```

### Schema Export Format

Used by:

- schema export
- schema import
- code generation

Schema export does not include schema ids or field ids. Regular schema create
requests do not accept client-provided ids, so imported schemas are persisted
with backend-generated ids.

Graph nodes still store `schemaId` in graph data. When importing graph data that
references schemas created in another project or environment, the editor should
resolve missing schema ids by `schemaTypeName` when possible and then save the
graph with current backend-generated schema ids.

```json
{
  "schemas": [
    {
      "schemaTypeName": "DialogueNode",
      "fields": [
        {
          "name": "speaker",
          "type": "string"
        }
      ]
    }
  ]
}
```

## Graph Contracts

Graphs describe nodes, edges, editor positions, and node data.

### Graph

Used by:

- graph import/export
- graph API create/update responses and requests
- runtime graph loading
- validation

```json
{
  "nodes": [
    {
      "id": "node-1",
      "type": "editableNode",
      "position": {
        "x": 100,
        "y": 200
      },
      "data": {
        "title": "Start",
        "schemaId": "6f3a2f1a-9b6d-4c5e-9b1a-123456789abc",
        "schemaTypeName": "DialogueNode",
        "properties": {
          "speaker": "Alice"
        }
      }
    },
    {
      "id": "node-2",
      "type": "editableNode",
      "position": {
        "x": 400,
        "y": 200
      },
      "data": {
        "title": "Reply",
        "schemaId": "6f3a2f1a-9b6d-4c5e-9b1a-123456789abc",
        "schemaTypeName": "DialogueNode",
        "properties": {
          "speaker": "Bob"
        }
      }
    }
  ],
  "edges": [
    {
      "id": "edge-1",
      "source": "node-1",
      "target": "node-2",
      "sourceHandle": "right",
      "targetHandle": "left"
    }
  ]
}
```

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| `nodes` | `GraphNode[]` | yes | Nodes contained in the graph. |
| `edges` | `GraphEdge[]` | yes | Directed edges between nodes. |

### Graph Node

```json
{
  "id": "node-1",
  "type": "editableNode",
  "position": {
    "x": 100,
    "y": 200
  },
  "data": {
    "title": "Start",
    "schemaId": "6f3a2f1a-9b6d-4c5e-9b1a-123456789abc",
    "schemaTypeName": "DialogueNode",
    "properties": {
      "speaker": "Alice"
    }
  }
}
```

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| `id` | `string` | yes | Node id, unique inside one graph. |
| `type` | `string` | yes | React Flow node view type. Usually `editableNode`. |
| `position` | `Position` | yes | Editor position. Runtime may preserve it for round-trip serialization. |
| `data` | `GraphNodeData` | yes | User-visible node data and schema-bound properties. |

### Graph Node Data

```json
{
  "title": "Start",
  "schemaId": "6f3a2f1a-9b6d-4c5e-9b1a-123456789abc",
  "schemaTypeName": "DialogueNode",
  "properties": {
    "speaker": "Alice",
    "priority": 3,
    "enabled": true
  }
}
```

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| `title` | `string` | yes | Display title of the node. |
| `schemaId` | `uuid` | yes | Stable schema id used to identify the schema for validation. |
| `schemaTypeName` | `string` | yes | Schema type name used for display and generated runtime classes. |
| `properties` | `object` | yes | Node values keyed by schema field name. |

`properties` is a dynamic object. Its keys should match field names from the
schema identified by `schemaId`.

### Position

```json
{
  "x": 100,
  "y": 200
}
```

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| `x` | `number` | yes | X coordinate in editor space. |
| `y` | `number` | yes | Y coordinate in editor space. |

### Graph Edge

```json
{
  "id": "edge-1",
  "source": "node-1",
  "target": "node-2",
  "sourceHandle": "right",
  "targetHandle": "left"
}
```

| Field | Type | Required | Description |
| --- | --- | --- | --- |
| `id` | `string` | yes | Edge id, unique inside one graph. |
| `source` | `string` | yes | Source node id. |
| `target` | `string` | yes | Target node id. |
| `sourceHandle` | `string` | yes | Source handle id. |
| `targetHandle` | `string` | yes | Target handle id. |

### Graph Save Request

Used by:

- `PUT /api/projects/{projectId}/graphs/{graphId}/content`
- saving graph nodes and edges without changing graph metadata

```json
{
  "content": {
    "nodes": [],
    "edges": []
  }
}
```

### Graph Update Request

Used by:

- `PUT /api/projects/{projectId}/graphs/{graphId}`
- updating graph metadata and graph data together

```json
{
  "name": "Dialogue graph",
  "content": {
    "nodes": [],
    "edges": []
  }
}
```

### Graph Response

Used by:

- `GET /api/projects/{projectId}/graphs/{graphId}`

```json
{
  "id": "01a071e8-1375-7393-8d90-5c490a83ad39",
  "projectId": "4de1cf43-bc7c-436e-85e1-24283ebd4bcc",
  "name": "Dialogue graph",
  "content": {
    "nodes": [],
    "edges": []
  }
}
```

## Validation Rules

### Schema Validation

- `schemaTypeName` must be present and non-empty.
- `schemaTypeName` must be unique inside a project.
- `schemaTypeName` must be in PascalCase, only letters and digits, starting with letter.
- `fields` must be present.
- field `id` must be present in API responses and update requests for existing fields.
- field `name` must be present and non-empty.
- field `name` must be present in camelCase, only letters and digits, starting with letter.
- field `name` must be unique inside one schema.
- field `type` must be one of `string`, `int`, `float`, or `bool`.

### Graph Validation

- `nodes` must be present.
- `edges` must be present.
- node `id` must be present and unique inside one graph.
- node `type` must be present.
- node `position` must be present.
- node `data` must be present.
- node `data.title` must be present.
- node `data.schemaId` must be present.
- node `data.schemaTypeName` must be present and non-empty.
- node `data.properties` must be an object.
- every edge `id` must be present and unique inside one graph.
- every edge `source` must reference an existing node id.
- every edge `target` must reference an existing node id.
- every edge `sourceHandle` must be present.
- every edge `targetHandle` must be present.
- when schema definitions are available, every node `schemaId` should reference an existing schema.
- when schema definitions are available, every key in `properties` should match a field in the referenced schema.
- when schema definitions are available, every property value should match the type declared by the referenced schema field.

## Runtime Notes

The runtime should load graph JSON into `GraphDto`, validate it, and then map
nodes to generated runtime classes using `schemaTypeName`.

Editor-only fields such as `position`, `type`, `sourceHandle`, and
`targetHandle` should be preserved during round-trip serialization even when
they are not used by runtime graph execution.
