export function createGraphSavePayload(nodes, edges) {
    return {
        nodes: nodes.map((node) => ({
            id: node.id,
            type: node.type,
            position: node.position,
            data: {
                label: node.data.label,
                type: node.data.type,
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