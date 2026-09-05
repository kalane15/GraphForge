import { useEffect, useState } from "react";
import { useParams } from "react-router";
import { getSchemasRequest } from "@/api/schemasApi";
import Schema from "./Schema";

function SchemasPage() {
    const { projectId } = useParams();
    const [schemas, setSchemas] = useState([]);

    function addSchemaField(schemaId, newFieldName, newFieldType) {
        setSchemas((schemas) =>
            schemas.map((schema) => {
                if (schema.id !== schemaId) {
                    return schema;
                }

                return {
                    ...schema,
                    fields: [
                        ...schema.fields,
                        {
                            id: crypto.randomUUID(),
                            name: newFieldName,
                            type: newFieldType,
                        },
                    ],
                };
            })
        );
    }

    function deleteSchemaField(schemaId, fieldId) {
        setSchemas((schemas) =>
            schemas.map((schema) => {
                if (schema.id !== schemaId) {
                    return schema;
                }

                return {
                    ...schema,
                    fields: schema.fields.filter((field) => field.id !== fieldId),
                };
            })
        );
    }

    function onSchemaChanged(schemaId, newSchema) {
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
            {schemas.map((schema) => (
                <div key={schema.id}>
                    <Schema
                        schemaId={schema.id}
                        schema={schema}
                        onChange={onSchemaChanged}
                    />
                </div>
            ))}
        </div>
    );
}

export default SchemasPage;
