export function createGraphSavePayload(nodes, edges) {
    return {
        nodes: nodes.map((node) => ({
            id: node.id,
            type: node.type,
            position: node.position,
            data: {
                title: node.data.title,
                schemaTypeName: node.data.schemaTypeName,
                properties: node.data.properties,
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
