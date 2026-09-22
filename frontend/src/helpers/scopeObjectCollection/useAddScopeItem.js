import { useCallback, useRef, useState } from "react";

export function useAddScopeItem({
    scopeKey,
    loadStatus,
    isSessionActive,
    setErrorMessage,
    appendItems,
    itemAdapter,
}) {
    const [isCreating, setIsCreating] = useState(false);
    const isCreatingRef = useRef(false);

    const addItem = useCallback(async (input) => {
        if (loadStatus !== "ready" || isCreatingRef.current || !isSessionActive()) {
            return false;
        }

        isCreatingRef.current = true;
        setIsCreating(true);

        try {
            const newItem = await itemAdapter.createItem(scopeKey, input);

            if (!isSessionActive()) {
                return false;
            }

            appendItems([newItem]);
            return true;
        } catch (error) {
            if (isSessionActive()) {
                setErrorMessage({ type: "error", text: `Error creating: ${error.message}` });
            }
            return false;
        } finally {
            isCreatingRef.current = false;
            if (isSessionActive()) {
                setIsCreating(false);
            }
        }
    }, [scopeKey, loadStatus, isSessionActive, setErrorMessage, appendItems, itemAdapter]);

    return { addItem, isCreating };
}
