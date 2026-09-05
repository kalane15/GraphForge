import SchemaElement from "./SchemaElement";


function Schema({ schemaId, schema, onChange }) {

    function updateSchemaFieldName(fieldId, newFieldName) {
        const newSchema = {
            ...schema,
            fields: schema.fields.map((field) => {
                if (field.id !== fieldId) {
                    return field;
                }

                return {
                    ...field,
                    name: newFieldName
                };
            }),
        };

        onChange(schemaId, newSchema);
    }

    function updateSchemaFieldType(fieldId, newFieldType) {
        const newSchema = {
            ...schema,
            fields: schema.fields.map((field) => {
                if (field.id !== fieldId) {
                    return field;
                }

                return {
                    ...field,
                    type: newFieldType
                };
            }),
        };

        onChange(schemaId, newSchema);
    }

    return (
        <div>
            <input className="editable-node__title" type="text" value={schema.schemaTypeName}></input> {
                schema.fields.map((field) => {
                    return (
                        <div className="editable-node__field-row" key={field.id}>
                            <SchemaElement
                                fieldId={field.id}
                                fieldName={field.name}
                                fieldType={field.type}
                                onFieldNameChange={updateSchemaFieldName}
                                onFieldTypeChange={updateSchemaFieldType} />
                        </div>
                    );
                })
            }
        </div>
    );
}

export default Schema;
