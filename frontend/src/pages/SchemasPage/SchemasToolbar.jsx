import { useRef } from "react";
import { useNavigate } from "react-router";


function SchemasToolbar({ addNewSchema, exportSchemas, importSchemas, projectId }) {
    const inputRef = useRef(null);
    const navigate = useNavigate();

    return (
        <div>
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

            <button onClick={() => navigate(`/projects/${projectId}`)}>
                Graphs
            </button>
        </div>
    );
}

export default SchemasToolbar;