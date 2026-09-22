import { useCallback, useState } from "react";

export function useDeleteScopeItem({
    setItems,
    session,
    isSessionActive,
    loadStatus,
    setErrorMessage,
    itemAdapter,
}) {
    const [deletingIds, setDeletingIds] = useState(new Set());

    const deleteItem = useCallback(async (itemId) => {
        if (loadStatus !== "ready" || !isSessionActive() || session.removedIds.has(itemId)) {
            return false;
        }

        // Block future autosaves before waiting for existing writes to finish.
        session.removedIds.add(itemId);
        setDeletingIds((current) => new Set([...current, itemId]));

        try {
            await session.queues.get(itemId)?.waitForIdle();
            if (!isSessionActive()) {
                return false;
            }

            await itemAdapter.deleteItem(session.scopeKey, itemId);

            if (!isSessionActive()) {
                return false;
            }

            session.queues.delete(itemId);
            setItems((current) => current.filter((item) => item.id !== itemId));
            return true;
        } catch (error) {
            session.removedIds.delete(itemId);
            if (isSessionActive()) {
                setErrorMessage({ type: "error", text: `Error deleting: ${error.message}` });
            }
            return false;
        } finally {
            if (isSessionActive()) {
                setDeletingIds((current) => new Set([...current].filter((id) => id !== itemId)));
            }
        }
    }, [setItems, session, isSessionActive, loadStatus, setErrorMessage, itemAdapter]);

    return { deleteItem, deletingIds };
}
