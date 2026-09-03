import { Handle, Position } from '@xyflow/react';

export function buildEditableNode({ label = "New node", position = { x: 0, y: 0 } } = {}) {
    return {
        id: crypto.randomUUID(),
        type: "editableNode",
        position,
        data: {
            label,
            "type": "dialogue",
            "properties": {
                "string": "asd"
            },
        },
    };
}



function getSchema(schemaName) {
    const schema = {
        "type": "dialogue",
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
            }
        ]
    }

    return schema
}



export function EditableNode({ id, data, selected }) {
    const properties = data.properties ?? {};
    const onChange = (name, value) => data.onFieldChange(id, name, value);

    function renderFieldInput(field, value) {
        if (field.type === "int") {
            return (
                <input
                    type="text"
                    value={value}
                    step="1"
                    onChange={(event) => handleIntChange(field.name, event)}
                />
            );
        }

        if (field.type === "float") {
            return (
                <input
                    type="text"
                    value={value}
                    step="any"
                    onChange={(event) => handleFloatChange(field.name, event)}
                />
            );
        }

        if (field.type === "bool") {
            return (
                <input
                    type="checkbox"
                    checked={value}
                    onChange={(event) => onChange(field.name, event.target.checked)}
                />
            );
        }

        return (
            <input
                type="text"
                value={value}
                onChange={(event) => onChange(field.name, event.target.value)}
            />
        );
    }

    function renderField(fieldSchema, data) {
        const value = data?.[fieldSchema.name] ?? "";

        return (
            <label>
                <span>{fieldSchema.name}</span>
                {renderFieldInput(fieldSchema, value, (newValue) => {
                    onChange(fieldSchema.name, newValue);
                })}
            </label>
        );
    }

    function handleFloatChange(name, event) {
        const value = event.target.value;

        if (!/^-?\d*\.?\d*$/.test(value)) {
            return;
        }

        onChange(name, value);
    }

    function handleIntChange(name, event) {
        const value = event.target.value;

        if (!/^-?\d*$/.test(value)) {
            return;
        }

        onChange(name, value);
    }

    return (
        <div className={`react-flow__node-default editable-node ${selected ? "editable-node--selected" : ""}`}>
            <Handle className="editable-node__handle" id="top" type="source" position={Position.Top} isConnectableStart isConnectableEnd />
            <Handle className="editable-node__handle" id="right" type="source" position={Position.Right} isConnectableStart isConnectableEnd />
            <Handle className="editable-node__handle" id="bottom" type="source" position={Position.Bottom} isConnectableStart isConnectableEnd />
            <Handle className="editable-node__handle" id="left" type="source" position={Position.Left} isConnectableStart isConnectableEnd />

            <div className="editable-node__title">{data.label}</div>
            {
                getSchema(data.type).fields.map((field) => {
                    return (
                        <div key={field.name}>
                            {renderField(field, properties)}
                        </div>
                    );
                })
            }
        </div>
    );
}
