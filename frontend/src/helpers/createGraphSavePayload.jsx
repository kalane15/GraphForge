import { parseFieldValue } from "./parseFieldValue";

function createNodeProperties(node, schemas) {
    const schema = schemas.find(
        (schema) => schema.id === node.data.schemaId
    );

    if (!schema) {
        throw new Error(`Schema "${node.data.schemaTypeName}" not found`);
    }

    const properties = node.data.properties ?? {};

    return Object.fromEntries(
        schema.fields.map((field) => [
            field.name,
            parseFieldValue(properties[field.name] ?? "", field.type, field.name)
        ])
    );
}

export function createGraphSavePayload(nodes, edges, schemas) {
    return {
        nodes: nodes.map((node) => ({
            id: node.id,
            type: node.type,
            position: node.position,
            data: {
                title: node.data.title,
                schemaId: node.data.schemaId,
                schemaTypeName: node.data.schemaTypeName,
                properties: createNodeProperties(node, schemas),
            },
        })),
        edges: edges.map((edge) => ({
            id: edge.id,
            source: edge.source,
            target: edge.target,
            sourceHandle: edge.sourceHandle,
            targetHandle: edge.targetHandle,
        })),
    };
}


