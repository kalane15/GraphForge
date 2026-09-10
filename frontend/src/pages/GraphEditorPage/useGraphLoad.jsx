import { useEffect, useState } from "react";
import { getGraphRequest } from "@/api/graphsApi";
import { getSchemasRequest } from "@/api/schemasApi";
import { mapSchemaToViewModel } from "@/helpers/schemaMappers";
import { resolveGraphSchemaReferences } from "./resolveGraphSchemaReferences";

export function useGraphLoad({ projectId, graphId, setSchemas, setNodes, setEdges }) {
    const [isGraphLoaded, setIsGraphLoaded] = useState(false);

    useEffect(() => {
        let isCancelled = false;

        async function loadGraph() {
            setIsGraphLoaded(false);

            const data = await getSchemasRequest(projectId);
            const loadedSchemas = (data?.schemas ?? [])
                .map(mapSchemaToViewModel);

            const graph = await getGraphRequest(graphId, projectId);
            const loadedNodes = resolveGraphSchemaReferences(
                graph.content.nodes,
                loadedSchemas
            );

            if (isCancelled) {
                return;
            }

            setSchemas(loadedSchemas);
            setNodes(loadedNodes);
            setEdges(graph.content.edges);
            setIsGraphLoaded(true);
        }

        loadGraph();

        return () => {
            isCancelled = true;
        };
    }, [projectId, graphId, setSchemas, setNodes, setEdges]);

    return isGraphLoaded;
}
