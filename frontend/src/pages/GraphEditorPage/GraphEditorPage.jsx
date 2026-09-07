import { useState, useEffect } from "react";
import Flow from "./Flow"
import { useNavigate, useParams } from "react-router"
import { ReactFlowProvider, Panel } from "@xyflow/react";
import { buildEditableNode } from "./EditableNode";
import { createGraphSavePayload } from "@/helpers/createGraphSavePayload";
import { updateGraphContentRequest, getGraphRequest } from "@/api/graphsApi";
import { getSchemasRequest } from "@/api/schemasApi";
import { downloadJsonFile } from "@/helpers/downloadJsonFile";


function GraphEditorPage() {
    const { projectId, graphId } = useParams();
    const navigate = useNavigate();
    const [schemas, setSchemas] = useState([]);

    const [nodes, setNodes] = useState(() => [
        buildEditableNode({
            label: "New Node",
            position: { x: 100, y: 100 },
        }),
        buildEditableNode({
            label: "New Node",
            position: { x: 150, y: 150 },
        }),
    ]);
    const [edges, setEdges] = useState([]);


    useEffect(() => {
        async function loadSchemas() {
            const data = await getSchemasRequest(projectId);
            const loadedSchemas = data?.schemas ?? [];

            setSchemas(loadedSchemas.map((schema) => ({
                id: schema.id,
                schemaTypeName: schema.schemaTypeName,
                fields: schema.content?.fields ?? [],
            })));
        }

        loadSchemas();
    }, [projectId]);
 

    useEffect(() => {
        async function loadGraph() {
            const graph = await getGraphRequest(graphId, projectId);

            setNodes(graph.content.nodes);
            setEdges(graph.content.edges);
        }

        loadGraph();
    }, [projectId, graphId]);

    useEffect(() => {
        const timeoutId = setTimeout(() => {
            saveGraph();
        }, 1000);

        return () => clearTimeout(timeoutId);
    }, [nodes, edges]);

    async function returnToProjectPage() {
        await saveGraph();
        navigate(`/projects/${projectId}`);
    }

    async function saveGraph() {
        const content = createGraphSavePayload(nodes, edges);
        await updateGraphContentRequest(graphId, projectId, content);
    }

    function goToSchemas() {
        navigate(`/projects/${projectId}/schemas`);
    }

    async function exportGraph() {
        const graph = await getGraphRequest(graphId, projectId);
        downloadJsonFile("graph.json", graph.content);
    }

    return (
        <div>
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
            </div>

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
    )
}

export default GraphEditorPage;
