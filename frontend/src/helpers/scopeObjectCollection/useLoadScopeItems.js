import { useCallback, useEffect, useState } from "react";

export function useLoadScopeItems({
    scopeKey,
    registerItems,
    itemAdapter,
    setErrorMessage,
    setItems,
}) {
    const [loadStatus, setLoadStatus] = useState("loading");
    const [loadAttempt, setLoadAttempt] = useState(0);

    useEffect(() => {
        let cancelled = false;

        async function loadItems() {
            setLoadStatus("loading");

            setErrorMessage(null);
            try {
                const loadedItems = await itemAdapter.getItemsList(scopeKey);

                if (!cancelled) {
                    registerItems(loadedItems);
                    setItems(loadedItems);
                    setLoadStatus("ready");
                }
            } catch (error) {
                if (!cancelled) {
                    setLoadStatus("error");
                    setErrorMessage({ type: "error", text: `Error loading items: ${error.message}` });
                }
            }
        }

        loadItems();
        return () => { cancelled = true; };

    }, [scopeKey, loadAttempt, registerItems, itemAdapter, setErrorMessage, setItems]);

    const reload = useCallback(() => {
        setLoadStatus("loading");
        setLoadAttempt((attempt) => attempt + 1);
    }, []);

    return { loadStatus, reload };
}
