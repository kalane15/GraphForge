const defaultFieldType = "string";
const defaultFieldName = "new field";

export function renameSchemaField(schema, fieldId, name) {
    return {
        ...schema,
        fields: schema.fields.map((field) => {
            if (field.id !== fieldId) {
                return field;
            }

            return {
                ...field,
                name,
            };
        }),
    };
}

export function changeSchemaFieldType(schema, fieldId, type) {
    return {
        ...schema,
        fields: schema.fields.map((field) => {
            if (field.id !== fieldId) {
                return field;
            }

            return {
                ...field,
                type,
            };
        }),
    };
}

export function addSchemaField(schema) {
    return {
        ...schema,
        fields: [
            ...schema.fields,
            {
                id: crypto.randomUUID(),
                name: getSchemaFieldDefaultName(schema.fields),
                type: defaultFieldType,
            },
        ],
    };
}

export function deleteSchemaField(schema, fieldId) {
    return {
        ...schema,
        fields: schema.fields.filter((field) => field.id !== fieldId),
    };
}

function getSchemaFieldDefaultName(fields) {
    let possibleName = defaultFieldName;
    let exists = fields.some((field) => field.name === possibleName);
    let number = 0;

    while (exists) {
        possibleName = defaultFieldName + " " + String(++number);
        exists = fields.some((field) => field.name === possibleName);
    }

    return possibleName;
}
