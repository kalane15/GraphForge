export function mapSchemaToViewModel(schema) {
    const fields = schema.fields ?? schema.content?.fields ?? schema.content?.Fields ?? [];

    return {
        id: schema.id ?? crypto.randomUUID(),
        schemaTypeName: schema.schemaTypeName,
        fields: fields.map((field) => ({
            id: field.id ?? crypto.randomUUID(),
            name: field.name,
            type: field.type,
        })),
    };
}