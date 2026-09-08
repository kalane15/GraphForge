import {
    addSchemaField,
    changeSchemaFieldType,
    deleteSchemaField,
    renameSchemaField
} from "@/helpers/schemaFields";
import SchemaFieldRow from "./SchemaFieldRow";


function Schema({ schemaId, schema, onSchemaUpdated, onSchemaDeleted }) {
    function updateSchemaFieldName(fieldId, newFieldName) {
        onSchemaUpdated(schemaId, renameSchemaField(schema, fieldId, newFieldName));
    }

    function updateSchemaFieldType(fieldId, newFieldType) {
        onSchemaUpdated(schemaId, changeSchemaFieldType(schema, fieldId, newFieldType));
    }

    function changeSchemaTypeName(newSchemaTypeName) {
        const newSchema = {
            ...schema,
            schemaTypeName: newSchemaTypeName
        };

        onSchemaUpdated(schemaId, newSchema);
    }

    function addField() {
        onSchemaUpdated(schemaId, addSchemaField(schema));
    }

    function deleteField(fieldId) {
        onSchemaUpdated(schemaId, deleteSchemaField(schema, fieldId));
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
                                <SchemaFieldRow
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
