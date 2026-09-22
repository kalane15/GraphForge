import { useScopeObjectCollectionOperations } from "@/helpers/scopeObjectCollection/useScopeObjectCollectionOperations.js";

export function useSchemaOperations({
    projectId,
    schemas,
    setSchemas,
    setMessage,
    adapter,
}) {
    const {
        saveItems,
        deleteItem,
        loadStatus,
        isCreating,
        isSaving,
        deletingIds,
        addItem,
        reload,
        appendItems,
    } = useScopeObjectCollectionOperations({
        scopeKey: projectId,
        items: schemas,
        setItems: setSchemas,
        setErrorMessage: setMessage,
        itemAdapter: adapter,
    });

    return {
        saveSchemas: saveItems,
        deleteSchema: deleteItem,
        loadStatus,
        isCreating,
        isSaving,
        deletingIds,
        addSchema: addItem,
        reload,
        appendSchemas: appendItems,
    };
}
