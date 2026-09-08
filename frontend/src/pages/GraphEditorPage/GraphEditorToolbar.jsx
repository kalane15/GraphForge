import { useRef } from "react";


function GraphEditorToolbar({ returnToProjectPage, goToSchemas, saveGraph, exportGraph, importGraph }) {
    const inputRef = useRef(null);

    return (
        <div className="graph-editor-toolbar">
            <button onClick={returnToProjectPage}>
                Return
            </button>

            <button onClick={goToSchemas}>
                Schemas
            </button>

            <button onClick={saveGraph}>
                Save
            </button>

            <button onClick={exportGraph}>
                Export
            </button>

            <button onClick={() => inputRef.current?.click()}>
                Import
            </button>

            <input
                ref={inputRef}
                type="file"
                accept=".json"
                hidden
                onChange={importGraph}
            />
        </div>
    )
}

export default GraphEditorToolbar;