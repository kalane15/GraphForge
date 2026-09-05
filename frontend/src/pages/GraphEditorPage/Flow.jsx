import { useCallback, useMemo, useRef } from 'react';
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
    useReactFlow,
    Panel
} from '@xyflow/react';

import '@xyflow/react/dist/style.css';
import './GraphEditorPage.css';
import { EditableNode, buildEditableNode } from "./EditableNode";
import { useCopyPaste } from "./useCopyPaste";

const nodeTypes = {
    editableNode: EditableNode,
};

function Flow({ nodes, edges, setNodes, setEdges, onSave, onReturn }) {
    const { screenToFlowPosition } = useReactFlow();
    const cursorPositionRef = useRef({ x: 0, y: 0 });

    const handleNodeFieldChange = useCallback((nodeId, fieldName, value) => {
        setNodes((nodes) =>
            nodes.map((node) => {
                if (node.id !== nodeId) {
                    return node;
                }

                return {
                    ...node,
                    data: {
                        ...node.data,
                        properties: {
                            ...node.data.properties,
                            [fieldName]: value,
                        },
                    },
                };
            })
        );
    }, [setNodes]);


    const nodesWithCallbacks = useMemo(() => nodes.map((node) => ({
        ...node,
        data: {
            ...node.data,
            onFieldChange: handleNodeFieldChange,
        },
    })), [nodes, handleNodeFieldChange]);

    useCopyPaste({ nodes, edges, setNodes, setEdges, cursorPositionRef });

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

                <Panel position="top-left">
                    <button onClick={onSave}> Save </button>
                    <button onClick={onReturn}> Return to project page </button>
                </Panel>
            </ReactFlow>
        </div>
    );
}

export default Flow;
