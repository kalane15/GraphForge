import { createSchemaRequest } from "@/api/schemasApi";
import { downloadJsonFile } from "@/helpers/downloadJsonFile";
import { mapSchemaToViewModel } from "@/helpers/schema/schemaMappers";

export function useImportExportSchemas({ projectId, schemas, onSchemasImported }) {
    function exportSchemas() {
        const exportData = {
            schemas: schemas.map((schema) => ({
                schemaTypeName: schema.schemaTypeName,
                fields: schema.fields.map((field) => ({
                    name: field.name,
                    type: field.type,
                })),
            })),
        };

        downloadJsonFile("schemas.schema.json", exportData);
    }

    async function importSchemas(event) {
        const input = event.target;
        const file = input.files[0];

        if (!file) {
            return;
        }

        try {
            const parsedFile = JSON.parse(await file.text());
            const schemasToImport = Array.isArray(parsedFile) ? parsedFile : parsedFile?.schemas;
            if (!Array.isArray(schemasToImport)) {
                throw new Error("Expected a schemas array");
            }

            const results = await Promise.allSettled(schemasToImport.map(async (schema) =>
                createSchemaRequest(projectId, schema.schemaTypeName, schema.fields)
            ));
            const importedSchemas = results
                .filter((result) => result.status === "fulfilled")
                .map((result) => mapSchemaToViewModel(result.value));
            onSchemasImported(importedSchemas);

            const failures = results.filter((result) => result.status === "rejected");
            if (failures.length > 0) {
                throw new Error(`${failures.length} schema(s) failed: ${failures.map((result) => result.reason.message).join("; ")}`);
            }
        } finally {
            input.value = "";
        }
    }

    return { exportSchemas, importSchemas };
}
