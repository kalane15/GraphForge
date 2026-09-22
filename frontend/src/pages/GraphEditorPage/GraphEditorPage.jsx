import { useCallback, useMemo, useState } from "react";
import Flow from "./Flow"
import { useNavigate, useParams } from "react-router"
import { ReactFlowProvider } from "@xyflow/react";
import GraphEditorToolbar from "./GraphEditorToolbar";
import { useImportExportGraph } from "./useImportExportGraph";
import { createGraphEditorItemAdapter } from "./graphEditorItemAdapter";
import { useGraphOperations } from "./useGraphOperations";

const emptyGraph = {
    nodes: [],
    edges: [],
    schemas: [],
};

function GraphEditorPage() {
    const { projectId, graphId } = useParams();
    return <ProjectGraphEditor key={`${projectId}:${graphId}`} projectId={projectId} graphId={graphId} />;
}

function ProjectGraphEditor({ projectId, graphId }) {
    const navigate = useNavigate();
    const [graphs, setGraphs] = useState([]);
    const [message, setMessage] = useState(null);
    const adapter = useMemo(() => createGraphEditorItemAdapter(), []);

    const {
        saveGraph,
        loadStatus,
        isSaving,
        reload,
    } = useGraphOperations({
        projectId,
        graphId,
        graphs,
        setGraphs,
        setMessage,
        adapter,
    });

    const graph = graphs[0] ?? emptyGraph;
    const { nodes, edges, schemas } = graph;

    const updateGraphPart = useCallback((part, valueOrUpdater) => {
        setGraphs((current) => {
            const currentGraph = current[0];
            if (!currentGraph) {
                return current;
            }

            const nextValue = typeof valueOrUpdater === "function"
                ? valueOrUpdater(currentGraph[part])
                : valueOrUpdater;

            if (nextValue === currentGraph[part]) {
                return current;
            }

            return [
                { ...currentGraph, [part]: nextValue },
                ...current.slice(1),
            ];
        });
    }, []);

    const setNodes = useCallback(
        (valueOrUpdater) => updateGraphPart("nodes", valueOrUpdater),
        [updateGraphPart]
    );
    const setEdges = useCallback(
        (valueOrUpdater) => updateGraphPart("edges", valueOrUpdater),
        [updateGraphPart]
    );

    const { importGraph, exportGraph } = useImportExportGraph({
        saveGraph,
        graphId,
        projectId,
        schemas,
        setNodes,
        setEdges,
    });

    async function returnToProjectPage() {
        const isSuccess = await saveGraph();
        if (isSuccess) {
            navigate(`/projects/${projectId}`);
        }
    }    

    async function goToSchemas() {
        const isSuccess = await saveGraph();
        if (isSuccess) {
            navigate(`/projects/${projectId}/schemas`);
        }
    }    

    return (
        <div className="graph-editor-page">
            <GraphEditorToolbar
                returnToProjectPage={returnToProjectPage}
                goToSchemas={goToSchemas}
                saveGraph={saveGraph}
                exportGraph={exportGraph}
                importGraph={importGraph}
                message={message}
                disabled={loadStatus !== "ready"}
                isSaving={isSaving}
            />

            {loadStatus === "loading" && <p role="status">Loading graph...</p>}
            {loadStatus === "error" && <button onClick={reload}>Retry</button>}
            {loadStatus === "ready" && (
                <div className="graph-editor-shell">
                    <ReactFlowProvider>
                        <Flow
                            nodes={nodes}
                            edges={edges}
                            setNodes={setNodes}
                            setEdges={setEdges}
                            schemas={schemas}
                        />
                    </ReactFlowProvider>
                </div>
            )}
        </div>
    )
}

export default GraphEditorPage;
