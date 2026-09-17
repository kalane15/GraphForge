import { useEffect, useState } from "react";
import { useParams } from "react-router";
import {
    deleteSchemaRequest,
    getSchemasRequest,
    updateSchemaRequest,
    createSchemaRequest
} from "@/api/schemasApi";
import Schema from "./Schema";
import { mapSchemaToViewModel } from "@/helpers/schemaMappers";
import SchemasToolbar from "./SchemasToolbar";
import { getSchemaDefaultName } from "@/helpers/schemaNames";
import { useImportExportSchemas } from "./useImportExportSchemas";


function SchemasPage() {
    const { projectId } = useParams();
    const [schemas, setSchemas] = useState([]);
    const { exportSchemas, importSchemas } = useImportExportSchemas({
        projectId,
        schemas,
        setSchemas
    });

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
        const response = await createSchemaRequest(projectId, getSchemaDefaultName(schemas), []);
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
