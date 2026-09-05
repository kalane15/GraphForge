export function createGraphSavePayload(nodes, edges) {
    return {
        nodes: nodes.map((node) => ({
            id: node.id,
            type: node.data.type,
            label: node.data.label,
            position: node.position,
            properties: node.data.properties,
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