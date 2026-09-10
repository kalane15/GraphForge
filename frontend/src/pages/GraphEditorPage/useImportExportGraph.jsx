import { useSaveGraph } from "./useSaveGraph";
import { getGraphRequest } from "@/api/graphsApi";
import { downloadJsonFile } from "@/helpers/downloadJsonFile";
import { resolveGraphSchemaReferences } from "./resolveGraphSchemaReferences";


export function useImportExportGraph(saveGraph, graphId, projectId, schemas, setNodes, setEdges) {
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

        const loadedNodes = resolveGraphSchemaReferences(graph.nodes, schemas);

        setNodes(loadedNodes);
        setEdges(graph.edges);
    };

    return { importGraph, exportGraph };
}