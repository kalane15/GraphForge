import { useCallback, useEffect, useMemo, useRef } from "react";
import { createSaveQueue } from "@/helpers/createSaveQueue";
import { useLoadScopeItems } from "./useLoadScopeItems";
import { useSaveScopeItems } from "./useSaveScopeItems";
import { useDeleteScopeItem } from "./useDeleteScopeItem";
import { useAddScopeItem } from "./useAddScopeItem";
export function useScopeObjectCollectionOperations({
    scopeKey,
    items,
    setItems,
    setErrorMessage,
    itemAdapter,
}) {
    const session = useMemo(() => ({
        scopeKey,
        queues: new Map(),
        removedIds: new Set(),
    }), [scopeKey]);
    const activeSessionRef = useRef(null);

    useEffect(() => {
        activeSessionRef.current = session;
        return () => { activeSessionRef.current = null; };
    }, [session]);

    const isSessionActive = useCallback(
        () => activeSessionRef.current === session,
        [session]
    );

    const registerItems = useCallback((savedItems) => {
        for (const item of savedItems) {
            session.queues.set(item.id, createSaveQueue(
                (input) => itemAdapter.updateItem(session.scopeKey, item.id, input),
                item
            ));
        }
    }, [session, itemAdapter]);

    const appendItems = useCallback((savedItems) => {
        if (!isSessionActive()) {
            return;
        }

        registerItems(savedItems);
        setItems((current) => [...current, ...savedItems]);
    }, [isSessionActive, registerItems, setItems]);

    const { loadStatus, reload } = useLoadScopeItems({
        scopeKey: session.scopeKey,
        registerItems,
        itemAdapter,
        setErrorMessage,
        setItems,
    });
    const { saveItems, isSaving } = useSaveScopeItems({
        items,
        session,
        isSessionActive,
        loadStatus,
        setErrorMessage,
        itemAdapter,
    });
    const { deleteItem, deletingIds } = useDeleteScopeItem({
        setItems,
        session,
        isSessionActive,
        loadStatus,
        setErrorMessage,
        itemAdapter,
    });
    const { addItem, isCreating } = useAddScopeItem({
        scopeKey: session.scopeKey,
        loadStatus,
        isSessionActive,
        setErrorMessage,
        appendItems,
        itemAdapter,
    });

    return {
        saveItems,
        deleteItem,
        loadStatus,
        isCreating,
        isSaving,
        deletingIds,
        addItem,
        reload,
        appendItems,
    };
}
