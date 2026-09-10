import { createGraphSavePayload } from "@/helpers/createGraphSavePayload";
import { updateGraphContentRequest } from "@/api/graphsApi";
import { useCallback, useEffect, useRef } from "react";


export function useSaveGraph(nodes, edges, projectId, graphId, setSaveStatusMessage, schemas, isGraphLoaded) {
    const messageTimeoutRef = useRef(null);


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
            return;
        }

        try {
            const content = createGraphSavePayload(nodes, edges, schemas);
            await updateGraphContentRequest(graphId, projectId, content);
            showMessage("Successfully saved", "success");
        } catch (ex) {
            showMessage(`Error during saving: ${ex.message}`, "error");
        }
    }, [nodes, edges, projectId, graphId, schemas, isGraphLoaded, showMessage]);


    useEffect(() => {
        if (!isGraphLoaded) {
            return;
        }

        const timeoutId = setTimeout(() => {
            saveGraph();
        }, 1000);

        return () => clearTimeout(timeoutId);
    }, [saveGraph, isGraphLoaded]);

    useEffect(() => {
        return () => {
            if (messageTimeoutRef.current) {
                clearTimeout(messageTimeoutRef.current);
            }
        };
    }, []);

    return saveGraph;
}
