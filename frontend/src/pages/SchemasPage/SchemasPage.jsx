import { useEffect, useRef, useState, useMemo } from "react";
import { useNavigate, useParams } from "react-router";
import Schema from "./Schema";
import SchemasToolbar from "./SchemasToolbar";
import { useImportExportSchemas } from "./useImportExportSchemas";
import "./SchemasPage.css";
import { createSchemasAdapter } from "./schemasItemAdapter.js"
import { useSchemaOperations } from "./useSchemaOperations.js"
import { getSchemaDefaultName } from "@/helpers/schema/schemaNames";


function SchemasPage() {
    const { projectId } = useParams();
    // Reset drafts and pending UI state when navigating between projects.
    return <ProjectSchemas key={projectId} projectId={projectId} />;
}

function ProjectSchemas({ projectId }) {
    const navigate = useNavigate();
    const [schemas, setSchemas] = useState([]);
    const [message, setMessage] = useState(null);

    const [isImporting, setIsImporting] = useState(false);
    const [isLeaving, setIsLeaving] = useState(false);

    const isMountedRef = useRef(false);

    const adapter = useMemo(() => {
        return createSchemasAdapter();
    }, []);

    const {
        saveSchemas,
        deleteSchema,
        loadStatus,
        isCreating,
        isSaving,
        deletingIds,
        addSchema,
        reload,
        appendSchemas,
    } = useSchemaOperations({
        projectId,
        schemas,
        setSchemas,
        setMessage,
        adapter,
    });

    const { exportSchemas, importSchemas } = useImportExportSchemas({
        projectId, schemas, onSchemasImported: appendSchemas
    });

    useEffect(() => {
        isMountedRef.current = true;        
        return () => { isMountedRef.current = false; };
    }, []);


    function onSchemaChanged(schemaId, newSchema) {
        setSchemas((current) => current.map((schema) => schema.id === schemaId ? newSchema : schema));
        setMessage((current) => current?.type === "success" ? null : current);
    }

    function addNewSchema() {
        return addSchema({
            schemaTypeName: getSchemaDefaultName(schemas),
            fields: [],
        });
    }

    async function handleImport(event) {
        setIsImporting(true);
        try {
            await importSchemas(event);
        } catch (error) {
            if (isMountedRef.current) {
                setMessage({ type: "error", text: `Error importing schemas: ${error.message}` });
            }
        } finally {
            if (isMountedRef.current) setIsImporting(false);
        }
    }

    async function returnToGraphs() {
        setIsLeaving(true);
        
        const success = await saveSchemas();
        if (!isMountedRef.current) return;
        if (success) {
            navigate(`/projects/${projectId}`);
        } else {
            setIsLeaving(false);
        }
    }

    
    return (
        <div className="schemas-page">
            <SchemasToolbar
                addNewSchema={addNewSchema}
                exportSchemas={exportSchemas}
                importSchemas={handleImport}
                returnToGraphs={returnToGraphs}
                saveSchemas={saveSchemas}
                disabled={loadStatus !== "ready" || isCreating || isImporting || isLeaving ||
                    deletingIds.size > 0}
                isSaving={isSaving}
            />
            {message && (
                <p className={`schemas-page__message schemas-page__message--${message.type}`}
                    role={message.type === "error" ? "alert" : "status"}>
                    {message.text}
                </p>
            )}
            {loadStatus === "loading" && <p role="status">Loading schemas...</p>}
            {loadStatus === "error" && (
                <button onClick={reload}>Retry</button>
            )}
            {schemas.map((schema) => (
                <fieldset className="schemas-page__schema" key={schema.id}
                    disabled={loadStatus !== "ready" || isLeaving || deletingIds.has(schema.id)}>
                    <Schema schemaId={schema.id} schema={schema}
                        onSchemaUpdated={onSchemaChanged} onSchemaDeleted={deleteSchema} />
                </fieldset>
            ))}
        </div>
    );
}

export default SchemasPage;
