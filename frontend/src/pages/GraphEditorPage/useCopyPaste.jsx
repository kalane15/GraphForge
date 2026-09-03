import { getNodesAveragePosition } from "./getNodesAveragePosition";
import { useCallback, useEffect, useRef } from "react";

export function useCopyPaste({
    nodes,
    edges,
    setNodes,
    setEdges,
    cursorPositionRef,
}) {
    const clipboardRef = useRef({ selectedNodes: [], selectedEdges: [], cursorPosition: { x: 0, y: 0 } });

    const copySelected = useCallback(() => {
        const selectedNodes = nodes.filter((node) => node.selected);
        const selectedNodeIds = new Set(selectedNodes.map((node) => node.id));

        const selectedEdges = edges.filter((edge) =>
            selectedNodeIds.has(edge.source) &&
            selectedNodeIds.has(edge.target)
        );

        clipboardRef.current = { selectedNodes, selectedEdges, cursorPosition: getNodesAveragePosition(selectedNodes) };
    }, [nodes, edges]);

    const pasteSelected = useCallback(() => {
        const idMap = new Map();

        const copiedNodes = clipboardRef.current.selectedNodes;
        const copiedEdges = clipboardRef.current.selectedEdges;

        if (copiedNodes.length === 0) {
            return;
        }

        pasteNodes(copiedNodes, idMap);
        pasteEdges(copiedEdges, idMap);
    }, []);

    function pasteNodes(copiedNodes, idMap) {
        const moveVector = {
            x: cursorPositionRef.current.x - clipboardRef.current.cursorPosition.x,
            y: cursorPositionRef.current.y - clipboardRef.current.cursorPosition.y
        };

        const pastedNodes = copiedNodes.map((node) => {
            const newId = crypto.randomUUID();
            idMap.set(node.id, newId);
            return {
                ...node,
                id: newId,
                selected: false,
                position: {
                    x: node.position.x + moveVector.x,
                    y: node.position.y + moveVector.y
                }
            }
        });

        setNodes((nodes) => {
            return [...nodes, ...pastedNodes];
        });
    }

    function pasteEdges(copiedEdges, idMap) {
        const pastedEdges = copiedEdges.map((edge) => ({
            ...edge,
            id: crypto.randomUUID(),
            source: idMap.get(edge.source),
            target: idMap.get(edge.target),
            selected: false
        }));

        setEdges((edges) => {
            return [...edges, ...pastedEdges];
        });
    }

    useEffect(() => {
        function handleKeyDown(event) {
            if (event.target.tagName === "INPUT" || event.target.tagName === "TEXTAREA") {
                return;
            }

            const isModifierPressed = event.ctrlKey || event.metaKey;

            const isCopy = isModifierPressed && event.code === "KeyC";
            const isPaste = isModifierPressed && event.code === "KeyV";

            if (isCopy) {
                event.preventDefault();
                copySelected();
            }

            if (isPaste) {
                event.preventDefault();
                pasteSelected();
            }
        }

        window.addEventListener("keydown", handleKeyDown);

        return () => {
            window.removeEventListener("keydown", handleKeyDown);
        };
    }, [copySelected, pasteSelected]);
}
