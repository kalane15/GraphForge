import { useState, useEffect } from "react";
import Flow from "./Flow"
import { useNavigate, useParams } from "react-router"
import { ReactFlowProvider } from "@xyflow/react";
import { buildEditableNode } from "./EditableNode";
import { createGraphSavePayload } from "@/helpers/createGraphSavePayload";
import { updateGraphContentRequest, getGraphRequest } from "@/api/graphsApi";


function GraphEditorPage() {
    const { projectId, graphId } = useParams();
    const navigate = useNavigate();

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
        async function loadGraph() {
            const graph = await getGraphRequest(graphId, projectId);

            setNodes(graph.content.nodes);
            setEdges(graph.content.edges);
        }

        loadGraph();
    }, [projectId, graphId]);

    async function returnToProjectPage() {
        await saveGraph();
        navigate(`/projects/${projectId}`);
    }

    async function saveGraph() {
        const content = createGraphSavePayload(nodes, edges);
        await updateGraphContentRequest(graphId, projectId, content);
    }

    return (
        <div>
            <ReactFlowProvider>
                <Flow
                    nodes={nodes}
                    edges={edges}
                    setNodes={setNodes}
                    setEdges={setEdges}
                    onReturn={returnToProjectPage}
                    onSave={saveGraph}
                />
            </ReactFlowProvider>
        </div>
    )
}

export default GraphEditorPage;
