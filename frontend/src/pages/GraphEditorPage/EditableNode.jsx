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
            "properties": {
                "string": "asd"
            },
        },
    };
}



function getSchema(schemaTypeName) {
    const schema = {
        schemaTypeName: "dialogue",
        "fields": [
            {
                "name": "string",
                "type": "string"
            },
            {
                "name": "float",
                "type": "float"
            },
            {
                "name": "int",
                "type": "int"
            },
            {
                "name": "bool",
                "type": "bool"
            },
            {
                "name": "isAboba",
                "type": "bool"
            }
        ]
    }

    return schema
}


export function EditableNode({ id, data, selected }) {
    const properties = data.properties ?? {};
    const onChange = (name, value) => data.onFieldChange(id, name, value);


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
            <div className="editable-node__type">schemaTypeName: {data.schemaTypeName}</div>
            <div className="editable-node__fields">
                {
                    getSchema(data.schemaTypeName).fields.map((field) => {
                        return (
                            <div className="editable-node__field-row" key={field.name}>
                                <EditableField
                                    name={field.name}
                                    value={properties[field.name] ?? ""}
                                    type={field.type}
                                    onChange={onChange}
                                    />
                            </div>
                        );
                    })
                }
            </div>
        </div>
    );
}
