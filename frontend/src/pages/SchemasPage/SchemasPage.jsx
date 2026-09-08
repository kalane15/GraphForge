import { useEffect, useState } from "react";
import { useParams } from "react-router";
import {
    createNoDataSchemaRequest,
    deleteSchemaRequest,
    getSchemasRequest,
    updateSchemaRequest,
    createSchemaWithFieldsRequest
} from "@/api/schemasApi";
import { downloadJsonFile } from "@/helpers/downloadJsonFile";
import Schema from "./Schema";
import { mapSchemaToViewModel } from "@/helpers/schemaMappers";
import SchemasToolbar from "./SchemasToolbar";
import { getSchemaDefaultName } from "@/helpers/schemaNames";


function SchemasPage() {
    const { projectId } = useParams();
    const [schemas, setSchemas] = useState([]);

    useEffect(() => {
        async function loadSchemas() {
            const data = await getSchemasRequest(projectId);
            const loadedSchemas = data?.schemas ?? [];

            setSchemas(loadedSchemas.map(mapSchemaToViewModel));
        }

        loadSchemas();
    }, [projectId]);

    async function onSchemaChanged(schemaId, newSchema) {
        await updateSchemaRequest(projectId, newSchema.id, newSchema);
        setSchemas((schemas) =>
            schemas.map((schema) => {
                if (schema.id !== schemaId) {
                    return schema;
                }

                return {
                    ...newSchema
                };
            })
        );
    }

    async function addNewSchema() {
        const response = await createNoDataSchemaRequest(projectId, getSchemaDefaultName(schemas));
        setSchemas
            (
                (schemas) => ([
                    ...schemas,
                    mapSchemaToViewModel(response)
                    
                ])
            );
    }

    async function onSchemaDeleted(schemaId) {
        await deleteSchemaRequest(projectId, schemaId);
        setSchemas(schemas.filter((schema) => schema.id != schemaId));
    }

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
                createSchemaWithFieldsRequest(
                    projectId,
                    schema.schemaTypeName,
                    schema.fields
                )
            )
        );        

        importedSchemas = importedSchemas.map(mapSchemaToViewModel)

        setSchemas((schemas) => ([...schemas, ...importedSchemas]));

        event.target.value = "";
    };

    
    return (
        <div>
            <SchemasToolbar
                addNewSchema={addNewSchema}
                exportSchemas={exportSchemas}
                importSchemas={importSchemas}
                projectId={projectId}
            />
                        
            {schemas.map((schema) => (
                <div key={schema.id}>
                    <Schema
                        schemaId={schema.id}
                        schema={schema}
                        onSchemaUpdated={onSchemaChanged}
                        onSchemaDeleted={onSchemaDeleted}
                    />
                </div>
            ))}
        </div>
    );
}

export default SchemasPage;
