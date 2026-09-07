import "@/pages/GraphEditorPage/GraphEditorPage.css"

function EditableField({ name, type, value, onChange }) {
    function renderFieldInput() {
        if (type === "int") {
            return (
                <input
                    className="editable-node__input"
                    type="text"
                    value={value}
                    step="1"
                    onChange={(event) => handleIntChange(name, event)}
                />
            );
        }

        if (type === "float") {
            return (
                <input
                    className="editable-node__input"
                    type="text"
                    value={value}
                    step="any"
                    onChange={(event) => handleFloatChange(name, event)}
                />
            );
        }

        if (type === "bool") {
            return (
                <input
                    className="editable-node__checkbox"
                    type="checkbox"
                    checked={value}
                    onChange={(event) => onChange(name, event.target.checked)}
                />
            );
        }

        return (
            <input
                className="editable-node__input"
                type="text"
                value={value}
                onChange={(event) => onChange(name, event.target.value)}
            />
        );
    }

    function renderField() {
        return (
            <label className="editable-node__field">
                <span className="editable-node__field-name">{name}</span>
                {renderFieldInput()}
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

    return renderField();
}

export default EditableField;
