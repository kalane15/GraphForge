import Flow from "./Flow"
import { useParams } from "react-router"
import { ReactFlowProvider } from "@xyflow/react";


function GraphEditorPage() {
    const { projectId, graphId } = useParams();

    return (
        <div>
            <ReactFlowProvider>
                <Flow />
            </ReactFlowProvider>
        </div>
    )
}

export default GraphEditorPage;