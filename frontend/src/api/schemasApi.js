import { request } from "./request"


export async function getSchemasRequest(projectId) {
    const path = `/projects/${projectId}/schemas`;

    return await request(path, {});
}

function createSchemaUpdatePayload(schema) {
    return {
        schemaTypeName: schema.schemaTypeName,        
        fields: schema.fields.map((field) => ({
            id: field.id,
            name: field.name,
            type: field.type,
        })),
    };
}

export async function createSchemaRequest(projectId, schemaTypeName, fields) {
    const path = `/projects/${projectId}/schemas`;
    const detail = {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
        },
        body: JSON.stringify({
            schemaTypeName: schemaTypeName,
            fields
        })
    }

    return await request(path, detail);
}

export async function updateSchemaRequest(projectId, schemaId, schema) {
    const path = `/projects/${projectId}/schemas/${schemaId}`;

    return await request(path, {
        method: "PUT",
        headers: {
            "Content-Type": "application/json",
        },

        body: JSON.stringify(createSchemaUpdatePayload(schema)),
    });
}

export async function deleteSchemaRequest(projectId, schemaId) {
    const path = `/projects/${projectId}/schemas/${schemaId}`;

    return await request(path, {method: "DELETE"});
}
