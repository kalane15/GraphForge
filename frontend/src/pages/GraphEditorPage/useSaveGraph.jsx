import { createGraphSavePayload } from "@/helpers/createGraphSavePayload";
import { updateGraphContentRequest } from "@/api/graphsApi";
import { useCallback, useEffect, useMemo, useRef } from "react";
import { createSaveQueue } from "@/helpers/createSaveQueue";
import { startAutosave } from "@/helpers/startAutosave";


export function useSaveGraph(nodes, edges, projectId, graphId, setSaveStatusMessage, schemas, isGraphLoaded) {
    const messageTimeoutRef = useRef(null);
    const saveGraphRef = useRef(null);
    const draftVersionRef = useRef(0);
    const activeQueueRef = useRef(null);
    const saveContent = useMemo(() => createSaveQueue(
        (content) => updateGraphContentRequest(graphId, projectId, content)
    ), [graphId, projectId]);

    useEffect(() => {
        activeQueueRef.current = saveContent;
        return () => {
            activeQueueRef.current = null;
        };
    }, [saveContent]);


    const showMessage = useCallback((text, type = "success") => {
        setSaveStatusMessage({ text, type });

        if (messageTimeoutRef.current) {
            clearTimeout(messageTimeoutRef.current);
        }

        messageTimeoutRef.current = setTimeout(() => {
            setSaveStatusMessage(null);
            messageTimeoutRef.current = null;
        }, 3000);
    }, [setSaveStatusMessage]);


    const saveGraph = useCallback(async () => {
        if (!isGraphLoaded) {
            return false;
        }

        const draftVersion = draftVersionRef.current;
        try {
            const content = createGraphSavePayload(nodes, edges, schemas);
            const wasWritten = await saveContent(content);
            if (wasWritten && activeQueueRef.current === saveContent && draftVersionRef.current === draftVersion) {
                showMessage("Successfully saved", "success");
            }
            return true;
        } catch (ex) {
            if (activeQueueRef.current === saveContent) {
                showMessage(`Error during saving: ${ex.message}`, "error");
            }
            return false;
        }
    }, [nodes, edges, schemas, isGraphLoaded, showMessage, saveContent]);

    useEffect(() => {
        saveGraphRef.current = saveGraph;
        draftVersionRef.current += 1;
    }, [saveGraph]);

    useEffect(() => {
        if (!isGraphLoaded) {
            return;
        }

        return startAutosave(() => saveGraphRef.current());
    }, [projectId, graphId, isGraphLoaded]);

    useEffect(() => {
        return () => {
            if (messageTimeoutRef.current) {
                clearTimeout(messageTimeoutRef.current);
            }
        };
    }, []);

    return saveGraph;
}
