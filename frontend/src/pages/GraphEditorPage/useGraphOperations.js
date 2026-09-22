import { useMemo } from "react";
import { useScopeObjectCollectionOperations } from "@/helpers/scopeObjectCollection/useScopeObjectCollectionOperations";

export function useGraphOperations({
    projectId,
    graphId,
    graphs,
    setGraphs,
    setMessage,
    adapter,
}) {
    const scopeKey = useMemo(
        () => ({ projectId, graphId }),
        [projectId, graphId]
    );
    const {
        saveItems,
        loadStatus,
        isSaving,
        reload,
    } = useScopeObjectCollectionOperations({
        scopeKey,
        items: graphs,
        setItems: setGraphs,
        setErrorMessage: setMessage,
        itemAdapter: adapter,
    });

    return {
        saveGraph: saveItems,
        loadStatus,
        isSaving,
        reload,
    };
}
