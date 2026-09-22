export function mapSchemaToViewModel(schema) {
    const fields = schema.fields;

    return {
        id: schema.id,
        schemaTypeName: schema.schemaTypeName,
        fields: fields.map((field) => ({
            id: field.id,
            name: field.name,
            type: field.type,
        })),
    };
}