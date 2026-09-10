import { useState, useEffect, useRef } from "react";
import Flow from "./Flow"
import { useNavigate, useParams } from "react-router"
import { ReactFlowProvider } from "@xyflow/react";
import { getSchemasRequest } from "@/api/schemasApi";
import { downloadJsonFile } from "@/helpers/downloadJsonFile";
import { mapSchemaToViewModel } from "@/helpers/schemaMappers";
import GraphEditorToolbar from "./GraphEditorToolbar";
import { useSaveGraph } from "./useSaveGraph"
import { getGraphRequest } from "@/api/graphsApi";


function GraphEditorPage() {
    const { projectId, graphId } = useParams();
    const navigate = useNavigate();

    const [schemas, setSchemas] = useState([]);
    const [nodes, setNodes] = useState(() => []);
    const [edges, setEdges] = useState([]);
    const [saveStatucMessage, setSaveStatusMessage] = useState(null);
    
    const saveGraph = useSaveGraph(nodes, edges, projectId, graphId, setSaveStatusMessage);

    useEffect(() => {
        async function loadGraph() {
            const data = await getSchemasRequest(projectId);
            const loadedSchemas = (data?.schemas ?? [])
                .map(mapSchemaToViewModel);

            const graph = await getGraphRequest(graphId, projectId);

            const loadedNodes = graph.content.nodes.map((node) => {
                const contains = loadedSchemas.some(
                    schema => schema.id === node.data.schemaId
                );
                
                if (contains) {
                    return node;
                }

                const schema = loadedSchemas.find(
                    schema => schema.schemaTypeName === node.data.schemaTypeName
                );
                console.log(schema);
                if (!schema) {
                    return node;
                }

                return {
                    ...node,
                    data: {
                        ...node.data,
                        schemaId: schema.id
                    }                    
                };
            });

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
                message={saveStatucMessage}
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
