import { getGraphRequest, updateGraphContentRequest } from "@/api/graphsApi";
import { getSchemasRequest } from "@/api/schemasApi";
import { createGraphSavePayload } from "@/helpers/createGraphSavePayload";
import { mapSchemaToViewModel } from "@/helpers/schema/schemaMappers";
import { resolveGraphSchemaReferences } from "./resolveGraphSchemaReferences";

export function createGraphEditorItemAdapter() {
    return {
        async getItemsList({ projectId, graphId }) {
            const [schemasData, graph] = await Promise.all([
                getSchemasRequest(projectId),
                getGraphRequest(graphId, projectId),
            ]);
            const schemas = (schemasData?.schemas ?? []).map(mapSchemaToViewModel);

            return [{
                id: graph.id,
                name: graph.name,
                schemas,
                nodes: resolveGraphSchemaReferences(graph.content.nodes, schemas),
                edges: graph.content.edges,
            }];
        },

        async updateItem({ projectId }, graphId, graph) {
            const content = createGraphSavePayload(graph.nodes, graph.edges, graph.schemas);
            await updateGraphContentRequest(graphId, projectId, content);
        },

        getLabel(graph) {
            return graph.name;
        },
    };
}
