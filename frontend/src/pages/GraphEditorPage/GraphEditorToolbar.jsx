import { useRef } from "react";


function GraphEditorToolbar({
    returnToProjectPage,
    goToSchemas,
    saveGraph,
    exportGraph,
    importGraph,
    message,
    disabled,
    isSaving,
}) {
    const inputRef = useRef(null);

    return (
        <div className="graph-editor-toolbar">
            <button onClick={returnToProjectPage} disabled={disabled}>
                Return
            </button>

            <button onClick={goToSchemas} disabled={disabled}>
                Schemas
            </button>

            <button onClick={saveGraph} disabled={disabled || isSaving}>
                {isSaving ? "Saving..." : "Save"}
            </button>

            <button onClick={exportGraph} disabled={disabled}>
                Export
            </button>

            <button onClick={() => inputRef.current?.click()} disabled={disabled}>
                Import
            </button>

            <input
                ref={inputRef}
                type="file"
                accept=".json"
                hidden
                disabled={disabled}
                onChange={importGraph}
            />
            {message && (
                <span className={`graph-editor-toolbar__message graph-editor-toolbar__message--${message.type}`}
                    role={message.type === "error" ? "alert" : "status"}>
                    {message.text}
                </span>
            )}
        </div>
    )
}

export default GraphEditorToolbar;
