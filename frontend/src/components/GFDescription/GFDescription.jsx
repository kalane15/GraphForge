import "@/styles/common.css";

const mainPageDescription = `
    GraphForge is a visual editor for creating and managing structured graphs.

    Design nodes, connect them, define custom data, and export your graphs for use in your applications. From dialogue systems and quest trees to workflows and game logic, GraphForge provides a flexible foundation without tying you to a specific use case.

    Create visually. Export easily. Integrate with C#.
`;

function MainPageDescription() {
    return (
        <div className="centered-container border-container">
            <h2>{ mainPageDescription }</h2>
        </div>
    );
}

export default MainPageDescription;