import { createGraphSavePayload } from "@/helpers/createGraphSavePayload";
import { updateGraphContentRequest, getGraphRequest } from "@/api/graphsApi";
import { useEffect } from "react";


function useSaveGraph(nodes, edges, projectId, graphId, setSaveStatusMessage) {
    const messageTimeoutRef = useRef(null);
    function showMessage(text, type = "success") {
        setSaveStatusMessage({ text, type });

        if (messageTimeoutRef.current) {
            clearTimeout(messageTimeoutRef.current);
        }

        messageTimeoutRef.current = setTimeout(() => {
            setSaveStatusMessage(null);
            messageTimeoutRef.current = null;
        }, 3000);
    }

    async function saveGraph() {
        try {
            const content = createGraphSavePayload(nodes, edges, schemas);
            await updateGraphContentRequest(graphId, projectId, content);
            showMessage("Successfully saved", "success");
        } catch (ex) {
            showMessage(`Error during saving: ${ex.message}`, "error");
        }
    }

    useEffect(() => {
        const timeoutId = setTimeout(() => {
            saveGraph();
        }, 1000);

        return () => clearTimeout(timeoutId);
    }, [nodes, edges]);
}