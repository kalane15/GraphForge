import { mapSchemaToViewModel } from "@/helpers/schema/schemaMappers";
import {
    createSchemaRequest,
    deleteSchemaRequest,
    getSchemasRequest,
    updateSchemaRequest
} from "@/api/schemasApi";


export function createSchemasAdapter() {
    return {
        async getItemsList(projectId) {
            const data = await getSchemasRequest(projectId);
            const loadedSchemas = (data?.schemas ?? []).map(mapSchemaToViewModel);
            return loadedSchemas;
        },

        async updateItem(projectId, schemaId, input) {
            await updateSchemaRequest(projectId, schemaId, input);      
        },

        async deleteItem(projectId, schemaId) {
            await deleteSchemaRequest(projectId, schemaId);
        },

        async createItem(projectId, { schemaTypeName, fields = [] }) {
            const response = await createSchemaRequest(projectId, schemaTypeName, fields);
            return mapSchemaToViewModel(response);
        },

        getLabel(schema) {
            return schema.schemaTypeName;
        }
    };
}
