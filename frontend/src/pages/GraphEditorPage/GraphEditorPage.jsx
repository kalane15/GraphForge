import { useState, useEffect, useRef } from "react";
import Flow from "./Flow"
import { useNavigate, useParams } from "react-router"
import { ReactFlowProvider } from "@xyflow/react";
import { getSchemasRequest } from "@/api/schemasApi";
import { mapSchemaToViewModel } from "@/helpers/schemaMappers";
import GraphEditorToolbar from "./GraphEditorToolbar";
import { useSaveGraph } from "./useSaveGraph"
import { getGraphRequest } from "@/api/graphsApi";
import { resolveGraphSchemaReferences } from "./resolveGraphSchemaReferences";
import { useImportExportGraph } from "./useImportExportGraph";


function GraphEditorPage() {
    const { projectId, graphId } = useParams();
    const navigate = useNavigate();

    const [schemas, setSchemas] = useState([]);
    const [nodes, setNodes] = useState(() => []);
    const [edges, setEdges] = useState([]);
    const [saveStatusMessage, setSaveStatusMessage] = useState(null);
    
    const saveGraph = useSaveGraph(nodes, edges, projectId, graphId, setSaveStatusMessage, schemas);
    const { importGraph, exportGraph } = useImportExportGraph();

    useEffect(() => {
        async function loadGraph() {
            const data = await getSchemasRequest(projectId);
            const loadedSchemas = (data?.schemas ?? [])
                .map(mapSchemaToViewModel);

            const graph = await getGraphRequest(graphId, projectId);

            const loadedNodes = resolveGraphSchemaReferences(graph.content.nodes, loadedSchemas);

            setSchemas(loadedSchemas);
            setNodes(loadedNodes);
            setEdges(graph.content.edges);
        }

        loadGraph();
    }, [projectId, graphId]);

    async function returnToProjectPage() {
        await saveGraph();
        navigate(`/projects/${projectId}`);
    }    

    async function goToSchemas() {
        await saveGraph();
        navigate(`/projects/${projectId}/schemas`);
    }    

    return (
        <div className="graph-editor-page">
            <GraphEditorToolbar
                returnToProjectPage={returnToProjectPage}
                goToSchemas={goToSchemas}
                saveGraph={saveGraph}
                exportGraph={exportGraph}
                importGraph={importGraph}
                message={saveStatusMessage}
            />

            <div className="graph-editor-shell">
                <ReactFlowProvider>
                    <Flow
                        nodes={nodes}
                        edges={edges}
                        setNodes={setNodes}
                        setEdges={setEdges}
                        projectId={projectId}
                        schemas={schemas}                        
                    />
                </ReactFlowProvider>
            </div>
        </div>
    )
}

export default GraphEditorPage;
