import { useState, useEffect, useRef } from "react";
import Flow from "./Flow"
import { useNavigate, useParams } from "react-router"
import { ReactFlowProvider } from "@xyflow/react";
import { createGraphSavePayload } from "@/helpers/createGraphSavePayload";
import { updateGraphContentRequest, getGraphRequest } from "@/api/graphsApi";
import { getSchemasRequest } from "@/api/schemasApi";
import { downloadJsonFile } from "@/helpers/downloadJsonFile";
import { mapSchemaToViewModel } from "@/helpers/schemaMappers";
import GraphEditorToolbar from "./GraphEditorToolbar";


function GraphEditorPage() {
    const { projectId, graphId } = useParams();
    const navigate = useNavigate();

    const [schemas, setSchemas] = useState([]);
    const [nodes, setNodes] = useState(() => []);
    const [edges, setEdges] = useState([]);
    const [message, setMessage] = useState("");


    useEffect(() => {
        async function loadSchemas() {
            const data = await getSchemasRequest(projectId);
            const loadedSchemas = data?.schemas ?? [];

            setSchemas(loadedSchemas.map(mapSchemaToViewModel));
        }        

        async function loadGraph() {
            await loadSchemas();

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

    useEffect(() => {
        return () => {
            if (messageTimeoutRef.current) {
                clearTimeout(messageTimeoutRef.current);
            }
        };
    }, []);

    const messageTimeoutRef = useRef(null);
    function showMessage(text) {
        setMessage(text);

        if (messageTimeoutRef.current) {
            clearTimeout(messageTimeoutRef.current);
        }

        messageTimeoutRef.current = setTimeout(() => {
            setMessage("");
            messageTimeoutRef.current = null;
        }, 3000);
    }

    async function returnToProjectPage() {
        await saveGraph();
        navigate(`/projects/${projectId}`);
    }

    async function saveGraph() {
        try {
            const content = createGraphSavePayload(nodes, edges, schemas);
            await updateGraphContentRequest(graphId, projectId, content);
            showMessage("Successfully saved")
        } catch (ex) {
            showMessage(`Error during saving: ${ex.message}`);
        }
    }

    async function goToSchemas() {
        await saveGraph();
        navigate(`/projects/${projectId}/schemas`);
    }

    async function exportGraph() {
        await saveGraph();

        const graph = await getGraphRequest(graphId, projectId);
        downloadJsonFile("graph.json", graph.content);
    }

    async function importGraph(event) {
        const file = event.target.files[0];

        if (!file) {
            return;
        }

        const text = await file.text();
        const graph = JSON.parse(text);

        setNodes(graph.nodes);
        setEdges(graph.edges);
    };
    

    return (
        <div className="graph-editor-page">
            <GraphEditorToolbar
                returnToProjectPage={returnToProjectPage}
                goToSchemas={goToSchemas}
                saveGraph={saveGraph}
                exportGraph={exportGraph}
                importGraph={importGraph}
                message={message}
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
