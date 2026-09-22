import { useCallback, useEffect, useRef, useState } from "react";
import { startAutosave } from "@/helpers/startAutosave";

export function useSaveScopeItems({
    items,
    session,
    isSessionActive,
    loadStatus,
    setErrorMessage,
    itemAdapter,
}) {
    const [isSaving, setIsSaving] = useState(false);
    const saveRef = useRef(null);
    const draftVersionRef = useRef(0);
    const pendingSavesRef = useRef(0);

    const saveItems = useCallback(async ({ silent = false } = {}) => {
        if (loadStatus !== "ready" || !isSessionActive()) {
            return false;
        }

        const draftVersion = draftVersionRef.current;
        const drafts = items.filter((item) => !session.removedIds.has(item.id));
        pendingSavesRef.current += 1;
        setIsSaving(true);

        try {
            const results = await Promise.allSettled(drafts.map((item) => {
                const save = session.queues.get(item.id);
                return save(item);
            }));

            if (!isSessionActive()) {
                return false;
            }

            const errors = results.flatMap((result, index) =>
                result.status === "rejected"
                    ? [`${itemAdapter.getLabel(drafts[index])}: ${result.reason.message}`]
                    : []
            );

            if (errors.length > 0) {
                setErrorMessage({ type: "error", text: `Error saving: ${errors.join("; ")}` });
                return false;
            }

            if (draftVersionRef.current !== draftVersion) {
                return false;
            }

            if (!silent || results.some((result) => result.value)) {
                setErrorMessage({ type: "success", text: "Saved" });
            }
            return true;
        } catch (error) {
            if (isSessionActive()) {
                setErrorMessage({ type: "error", text: `Error saving: ${error.message}` });
            }
            return false;
        } finally {
            pendingSavesRef.current -= 1;
            if (isSessionActive()) {
                setIsSaving(pendingSavesRef.current > 0);
            }
        }
    }, [items, session, isSessionActive, loadStatus, itemAdapter, setErrorMessage]);

    useEffect(() => {
        saveRef.current = saveItems;
        draftVersionRef.current += 1;
    }, [saveItems]);

    useEffect(() => {
        if (loadStatus === "ready") {
            return startAutosave(() => saveRef.current({ silent: true }));
        }
    }, [session, loadStatus]);

    return { saveItems, isSaving };
}
