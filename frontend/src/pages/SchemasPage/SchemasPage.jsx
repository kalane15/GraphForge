import { useEffect, useState , useRef } from "react";
import { useParams, useNavigate } from "react-router";
import {
    createNoDataSchemaRequest,
    deleteSchemaRequest,
    getSchemasRequest,
    updateSchemaRequest,
    createSchemaWithContentRequest
} from "@/api/schemasApi";
import { downloadJsonFile } from "@/helpers/downloadJsonFile";
import Schema from "./Schema";

function SchemasPage() {
    const { projectId } = useParams();
    const [schemas, setSchemas] = useState([]);
    const schemaDefaultName = "New schema";
    const navigate = useNavigate();

    function mapSchemaToViewModel(schema) {
        const fields = schema.fields ?? schema.content?.fields ?? schema.content?.Fields ?? [];

        return {
            id: schema.id ?? crypto.randomUUID(),
            schemaTypeName: schema.schemaTypeName,
            fields: fields.map((field) => ({
                id: field.id ?? crypto.randomUUID(),
                name: field.name,
                type: field.type,
            })),
        };
    }

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
        const response = await createNoDataSchemaRequest(projectId, getSchemaDefaultName());
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

    useEffect(() => {
        async function loadSchemas() {
            const data = await getSchemasRequest(projectId);
            const loadedSchemas = data?.schemas ?? [];

            setSchemas(loadedSchemas.map(mapSchemaToViewModel));
        }

        loadSchemas();
    }, [projectId]);

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

        importedSchemas = importedSchemas.map((schema) => ({
            schemaTypeName: schema.schemaTypeName,
            fields: (schema.fields ?? schema.content?.fields ?? []).map((field) => ({
                name: field.name,
                type: field.type,
            }))
        }));

        importedSchemas = await Promise.all(
            importedSchemas.map((schema) =>
                createSchemaWithContentRequest(
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

    const inputRef = useRef(null);

    return (
        <div>
            <button onClick={ addNewSchema } >Add new schema</button>
            <button onClick={exportSchemas} >Export</button>

            <button onClick={() => inputRef.current?.click()}>
                Import
            </button>

            <input
                ref={inputRef}
                type="file"
                accept=".json"
                hidden
                onChange={importSchemas}
            />


            <button onClick={() => navigate(`/projects/${projectId}`)}>
                Graphs
            </button>

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
