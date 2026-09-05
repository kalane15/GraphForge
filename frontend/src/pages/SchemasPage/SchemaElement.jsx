import schemaFieldTypesAvailable from "@/schemaFieldTypesAvailable";


function SchemaElement({ fieldId, fieldName, fieldType, onFieldNameChange, onFieldTypeChange }) {

    return (
        <div className="editable-node__field-row">
            <input type="text" value={fieldName} onChange={(event) => onFieldNameChange(fieldId, event.target.value) } ></input>
            <select
                value={fieldType}
                onChange={(event) => onFieldTypeChange(fieldId, event.target.value) }>

                {schemaFieldTypesAvailable.map((fieldType) => (
                    <option key={fieldType} value={fieldType}>
                        {fieldType}
                    </option>
                ))}
            </select>
        </div>
    )
}

export default SchemaElement;
