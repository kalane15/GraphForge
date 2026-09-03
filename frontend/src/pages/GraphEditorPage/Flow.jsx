import { useState, useCallback, useMemo, useRef } from 'react';
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



const initialEdges = [];

const nodeTypes = {
    editableNode: EditableNode,
};

function Flow() {
    const { screenToFlowPosition } = useReactFlow();
    const [nodes, setNodes] = useState(() => [
        buildEditableNode({
            label: "New Node",
            position: { x: 100, y: 100 },
        }),
        buildEditableNode({
            label: "New Node",
            position: { x: 150, y: 150 },
        }),
    ]);
    const [edges, setEdges] = useState(initialEdges);
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
    }, []);

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
    }, [screenToFlowPosition]);



    const handleMouseMove = useCallback((event) => {
        cursorPositionRef.current = screenToFlowPosition({
            x: event.clientX,
            y: event.clientY,
        });
    }, [screenToFlowPosition]);

    const onConnect = useCallback((connection) => {
        setEdges((edges) => addEdge(connection, edges));
    }, []);

    const onNodesChange = useCallback(
        (changes) => setNodes((nds) => applyNodeChanges(changes, nds)),
        [],
    );
    const onEdgesChange = useCallback(
        (changes) => setEdges((eds) => applyEdgeChanges(changes, eds)),
        [],
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
