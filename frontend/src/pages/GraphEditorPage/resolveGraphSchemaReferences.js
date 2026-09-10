export function resolveGraphSchemaReferences(nodes, schemas){
    return nodes.map((node) => {
        const contains = schemas.some(
            schema => schema.id === node.data.schemaId
        );

        if (contains) {
            return node;
        }

        const schema = schemas.find(
            schema => schema.schemaTypeName === node.data.schemaTypeName
        );

        if (!schema) {
            return node;
        }

        return {
            ...node,
            data: {
                ...node.data,
                schemaId: schema.id
            }
        };
    });
}