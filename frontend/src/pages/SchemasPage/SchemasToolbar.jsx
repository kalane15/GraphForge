import { useRef } from "react";


function SchemasToolbar({ addNewSchema, exportSchemas, importSchemas, returnToGraphs, saveSchemas, disabled, isSaving }) {
    const inputRef = useRef(null);

    return (
        <fieldset className="schemas-page__toolbar" disabled={disabled}>
            <button onClick={addNewSchema} >Add new schema</button>
            <button onClick={exportSchemas} >Export</button>

            <button onClick={() => inputRef.current?.click()}>
                Import
            </button>

            <input
                ref={inputRef}
                type="file"
                accept=".json"
                hidden
                onChange={importSchemas}
            />

            <button onClick={returnToGraphs}>
                Graphs
            </button>

            <button onClick={() => saveSchemas()} disabled={isSaving}>
                {isSaving ? "Saving..." : "Save schemas"}
            </button>
        </fieldset>
    );
}

export default SchemasToolbar;
