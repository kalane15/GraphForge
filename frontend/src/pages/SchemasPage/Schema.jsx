import SchemaElement from "./SchemaElement";


function Schema({ schemaId, schema, onSchemaUpdated, onSchemaDeleted }) {
    const schemaFieldDefaultType = "string";
    const schemaFieldDefaultName = "new field";


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

        onSchemaUpdated(schemaId, newSchema);
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

        onSchemaUpdated(schemaId, newSchema);
    }

    function changeSchemaTypeName(newSchemaTypeName) {
        const newSchema = {
            ...schema,
            schemaTypeName: newSchemaTypeName
        };

        onSchemaUpdated(schemaId, newSchema);
    }


    function getFieldDefaultName() {
        const fields = schema.fields;
        let possibleName = schemaFieldDefaultName;
        let exists = fields.some(field => field.name === possibleName);
        let number = 0;

        while (exists) {
            possibleName = schemaFieldDefaultName + " " + String(++number);
            exists = fields.some(field => field.name === possibleName);
        }

        return possibleName;
    }

    function addField() {
        const newSchema = {
            ...schema,
            fields: [
                ...schema.fields,
                {
                    id: crypto.randomUUID(),
                    name: getFieldDefaultName(),
                    type: schemaFieldDefaultType,
                },
            ],
        };

        onSchemaUpdated(schemaId, newSchema);
    }

    function deleteField(fieldId) {
        const newSchema = {
            ...schema,
            fields: schema.fields.filter((field) => field.id !== fieldId),
        };

        onSchemaUpdated(schemaId, newSchema);
    }

    function deleteSchema() {
        onSchemaDeleted(schemaId);
    }


    return (
        <div>
            <input
                className="editable-node__title"
                type="text"
                value={schema.schemaTypeName} onChange={(e) => changeSchemaTypeName(e.target.value)}>
            </input>
            <button onClick={addField}> Add new field </button>
            <button onClick={deleteSchema}> Delete schema </button>
            {
                schema.fields.map (
                    (field) => {
                        return (
                            <div className="editable-node__field-row" key={field.id}>
                                <SchemaElement
                                    fieldId={field.id}
                                    fieldName={field.name}
                                    fieldType={field.type}
                                    onFieldNameChange={updateSchemaFieldName}
                                    onFieldTypeChange={updateSchemaFieldType}
                                    onFieldDeleted={deleteField} />
                            </div>
                        );
                    }
                )                
            }
        </div>
    );
}

export default Schema;
