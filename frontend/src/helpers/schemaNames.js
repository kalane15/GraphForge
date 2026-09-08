const defaultSchemaName = "New schema";

export function getSchemaDefaultName(schemas) {
    let possibleName = defaultSchemaName;
    let exists = schemas.some((schema) => schema.schemaTypeName === possibleName);
    let number = 0;

    while (exists) {
        possibleName = defaultSchemaName + " " + String(++number);
        exists = schemas.some((schema) => schema.schemaTypeName === possibleName);
    }

    return possibleName;
}
