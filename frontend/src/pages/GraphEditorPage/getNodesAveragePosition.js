
export function getNodesAveragePosition(nodes) {
    if (nodes.length === 0) {
        return null;
    }

    const sum = nodes.reduce(
        (acc, node) => ({
            x: acc.x + node.position.x,
            y: acc.y + node.position.y,
        }),
        { x: 0, y: 0 }
    );

    return {
        x: sum.x / nodes.length,
        y: sum.y / nodes.length,
    };
}