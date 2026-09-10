import { createSchemaRequest } from "@/api/schemasApi";
import { downloadJsonFile } from "@/helpers/downloadJsonFile";
import { mapSchemaToViewModel } from "@/helpers/schemaMappers";

export function useImportExportSchemas({ projectId, schemas, setSchemas }) {
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
        const file = event.target.files[0];

        if (!file) {
            return;
        }

        const text = await file.text();
        const parsedFile = JSON.parse(text);
        let importedSchemas = Array.isArray(parsedFile)
            ? parsedFile
            : parsedFile?.schemas ?? [];

        importedSchemas = await Promise.all(
            importedSchemas.map((schema) =>
                createSchemaRequest(
                    projectId,
                    schema.schemaTypeName,
                    schema.fields
                )
            )
        );

        importedSchemas = importedSchemas.map(mapSchemaToViewModel);

        setSchemas((schemas) => ([...schemas, ...importedSchemas]));

        event.target.value = "";
    }

    return { exportSchemas, importSchemas };
}
