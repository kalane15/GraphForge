import { useEffect, useState } from "react";
import { useParams } from "react-router";
import {
    createSchemaRequest,
    deleteSchemaRequest,
    getSchemasRequest,
    updateSchemaRequest
} from "@/api/schemasApi";
import { downloadJsonFile } from "@/helpers/downloadJsonFile";
import Schema from "./Schema";

function SchemasPage() {
    const { projectId } = useParams();
    const [schemas, setSchemas] = useState([]);
    const schemaDefaultName = "New schema";


    function onSchemaChanged(schemaId, newSchema) {
        updateSchemaRequest(projectId, newSchema.id, newSchema)
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

    function getSchemaDefaultName() {
        let possibleName = schemaDefaultName;
        let exists = schemas.some(schema => schema.schemaTypeName === possibleName);
        let number = 0;

        while (exists) {
            possibleName = schemaDefaultName + " " + String(++number);
            exists = schemas.some(schema => schema.schemaTypeName === possibleName);
        }

        return possibleName;
    }

    async function addNewSchema() {
        const response = await createSchemaRequest(projectId, getSchemaDefaultName());
        setSchemas
            (
                (schemas) => ([
                    ...schemas,
                    {
                        id: response.id,
                        schemaTypeName: response.schemaTypeName,
                        fields: (response.content?.fields ?? []).map((field) => ({
                            id: field.id ?? crypto.randomUUID(),
                            name: field.name,
                            type: field.type,
                        })),
                    }
                    
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

        downloadJsonFile("schemas.json", exportData);
    }

    useEffect(() => {
        async function loadSchemas() {
            const data = await getSchemasRequest(projectId);
            const loadedSchemas = data?.schemas ?? [];

            setSchemas(loadedSchemas.map((schema) => ({
                id: schema.id,
                schemaTypeName: schema.schemaTypeName,
                fields: (schema.content?.fields ?? []).map((field) => ({
                    id: field.id ?? crypto.randomUUID(),
                    name: field.name,
                    type: field.type,
                })),
            })));
        }

        loadSchemas();
    }, [projectId]);



    return (
        <div>
            <button onClick={ addNewSchema } >Add new schema</button>
            <button onClick={ exportSchemas } >Export</button>
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
