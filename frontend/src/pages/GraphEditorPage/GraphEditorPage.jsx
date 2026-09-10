import { useState } from "react";
import Flow from "./Flow"
import { useNavigate, useParams } from "react-router"
import { ReactFlowProvider } from "@xyflow/react";
import GraphEditorToolbar from "./GraphEditorToolbar";
import { useSaveGraph } from "./useSaveGraph"
import { useImportExportGraph } from "./useImportExportGraph";
import { useGraphLoad } from "./useGraphLoad";


function GraphEditorPage() {
    const { projectId, graphId } = useParams();
    const navigate = useNavigate();

    const [schemas, setSchemas] = useState([]);
    const [nodes, setNodes] = useState(() => []);
    const [edges, setEdges] = useState([]);
    const [saveStatusMessage, setSaveStatusMessage] = useState(null);

    const isGraphLoaded = useGraphLoad({
        projectId,
        graphId,
        setSchemas,
        setNodes,
        setEdges
    });

    const saveGraph = useSaveGraph(
        nodes,
        edges,
        projectId,
        graphId,
        setSaveStatusMessage,
        schemas,
        isGraphLoaded
    );

    const { importGraph, exportGraph } = useImportExportGraph({
        saveGraph,
        graphId,
        projectId,
        schemas,
        setNodes,
        setEdges
    });

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
