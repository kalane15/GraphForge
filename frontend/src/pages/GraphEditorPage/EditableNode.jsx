import { Handle, Position } from '@xyflow/react';
import EditableField from "@/components/EditableField/EditableField"


export function buildEditableNode({ title = "New node", position = { x: 0, y: 0 } } = {}) {
    return {
        id: crypto.randomUUID(),
        type: "editableNode",
        position,
        data: {
            title,
            schemaTypeName: "dialogue",
            "properties": {},
        },
    };
}


export function EditableNode({ id, data, selected }) {
    const schemas = data.schemas ?? [];
    const properties = data.properties ?? {};
    const schema = schemas.find(
        (schema) => schema.schemaTypeName === data.schemaTypeName
    );

    const isSchemaMissing = schemas.length > 0 && !schema;



    function handleTitleChange(event) {
        const value = event.target.value;

        data.onTitleChange(id, event.target.value)
    }


    return (
        <div className={`react-flow__node-default editable-node ${selected ? "editable-node--selected" : ""}`}>
            <Handle className="editable-node__handle" id="top" type="source" position={Position.Top} isConnectableStart isConnectableEnd />
            <Handle className="editable-node__handle" id="right" type="source" position={Position.Right} isConnectableStart isConnectableEnd />
            <Handle className="editable-node__handle" id="bottom" type="source" position={Position.Bottom} isConnectableStart isConnectableEnd />
            <Handle className="editable-node__handle" id="left" type="source" position={Position.Left} isConnectableStart isConnectableEnd />

           

            <input className="editable-node__title" type="text" value={data.title} onChange={(event) => handleTitleChange(event)}></input>
            {isSchemaMissing && (
                <div className="editable-node__missing-schema">
                    Missing schema: {data.schemaTypeName}
                </div>
            )}
            <div className="editable-node__type">
                <span className="editable-node__type-label">schemaTypeName:</span>
                <select
                    className="editable-node__schema-select"
                    value={data.schemaTypeName}
                        onChange={(event) => data.onSchemaTypeChange(id, event.target.value)}>

                    {isSchemaMissing && (
                        <option key={"missing"} value={data.schemaTypeName}>
                            {data.schemaTypeName}
                        </option>
                    )}

                    {schemas.map((schema) => (
                        <option key={schema.id} value={schema.schemaTypeName}>
                            {schema.schemaTypeName}
                        </option>       
                    ))}



                </select>
            </div>

            <div className="editable-node__fields">
                {
                    (schema?.fields ?? []).map((field) => {
                        return (
                            <div className="editable-node__field-row" key={field.name}>
                                <EditableField
                                    name={field.name}
                                    value={properties[field.name] ?? ""}
                                    type={field.type}
                                    onChange={(name, value) => data.onFieldChange(id, name, value, field.type)}
                                    />
                            </div>
                        );
                    })
                }
            </div>
        </div>
    );
}
