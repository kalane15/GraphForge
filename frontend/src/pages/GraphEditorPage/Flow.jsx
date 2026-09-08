import { useCallback, useRef } from 'react';
import {
    ReactFlow,
    Controls,
    ControlButton,
    Background,
    applyNodeChanges,
    applyEdgeChanges,
    addEdge,
    SelectionMode,
    ConnectionMode,
    useReactFlow
} from '@xyflow/react';

import '@xyflow/react/dist/style.css';
import './GraphEditorPage.css';
import { EditableNode, buildEditableNode } from "./EditableNode";
import { useCopyPaste } from "./useCopyPaste";
import { useEditableNodesWithChangesCallbacks } from "./useEditableNodesWithChangesCallbacks"

const nodeTypes = {
    editableNode: EditableNode
};

function Flow({ nodes, edges, setNodes, setEdges, projectId, schemas }) {
    const { screenToFlowPosition } = useReactFlow();
    const cursorPositionRef = useRef({ x: 0, y: 0 });

    useCopyPaste({ nodes, edges, setNodes, setEdges, cursorPositionRef });

    const nodesWithCallbacks = useEditableNodesWithChangesCallbacks({ nodes, setNodes, schemas });

    const addNode = useCallback(() => {
        const position = screenToFlowPosition({
            x: window.innerWidth / 2,
            y: window.innerHeight / 2
        });

        const newNode = buildEditableNode({ position });

        setNodes((nodes) => [...nodes, newNode]);
    }, [screenToFlowPosition, setNodes]);


    const handleMouseMove = useCallback((event) => {
        cursorPositionRef.current = screenToFlowPosition({
            x: event.clientX,
            y: event.clientY,
        });
    }, [screenToFlowPosition]);

    const onConnect = useCallback((connection) => {
        setEdges((edges) => addEdge(connection, edges));
    }, [setEdges]);

    const onNodesChange = useCallback(
        (changes) => setNodes((nds) => applyNodeChanges(changes, nds)),
        [setNodes],
    );
    const onEdgesChange = useCallback(
        (changes) => setEdges((eds) => applyEdgeChanges(changes, eds)),
        [setEdges],
    );


    return (
        <div className="graph-editor">
            <ReactFlow
                nodes={nodesWithCallbacks}
                edges={edges}
                onNodesChange={onNodesChange}
                onEdgesChange={onEdgesChange}
                onConnect={onConnect}
                fitView
                colorMode="system"
                selectionOnDrag
                selectionMode={SelectionMode.Partial}
                panOnDrag={[1, 2]}
                multiSelectionKeyCode={["Shift", "Control", "Meta"]}
                nodeTypes={nodeTypes}
                connectionMode={ConnectionMode.Loose}
                deleteKeyCode="Delete"
                onMouseMove={handleMouseMove}>
                <Background />
                <Controls>
                    <ControlButton onClick={addNode} title="Add node" aria-label="Add node">
                        add
                    </ControlButton>
                </Controls>
            </ReactFlow>
        </div>
    );
}

export default Flow;
