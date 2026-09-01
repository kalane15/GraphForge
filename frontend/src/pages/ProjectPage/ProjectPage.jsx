import { useEffect, useState } from "react";
import { useParams } from "react-router";
import GraphCreationForm from "./GraphCreationForm";
import GraphEntry from "./GraphEntry";


function ProjectPage() {
    const { projectId } = useParams();

    const [project, setProject] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState(null);
    const [isCreateOpen, setIsCreateOpen] = useState(false);

    useEffect(() => {
        async function loadProject() {
            try {
                setIsLoading(true);
                setError(null);

                const response = await fetch(
                    `${import.meta.env.VITE_BACKEND_URL}/projects/${projectId}`,
                    {
                        method: "GET",
                        credentials: "include"
                    }
                );

                if (!response.ok) {
                    throw new Error("Failed to load project");
                }

                const data = await response.json();
                setProject(data);
            }
            catch (error) {
                setError(error.message);
            }
            finally {
                setIsLoading(false);
            }
        }

        loadProject();
    }, []);

    async function createGraph(name) {
        const response = await fetch(
            `${import.meta.env.VITE_BACKEND_URL}/graphs`,
            {
                method: "POST",
                credentials: "include",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ name, projectId })
            }
        );

        if (!response.ok) {
            const errorData = await response.json();
            throw new Error(errorData.message);
        }

        const graph = await response.json();
        setProject({ ...project, graphs: [...project.graphs, graph] });
    }

    if (isLoading) {
        return <div>Loading...</div>;
    }

    if (error) {
        return <div>{error}</div>;
    }

    if (!project) {
        return <div>Project not found</div>;
    }

    return (
        <div>
            <h1>{project.name}</h1>
            <p>{project.description}</p>

            <h2>Graphs</h2>

            <button onClick={() => setIsCreateOpen(true)}>
                Create graph
            </button>
            {isCreateOpen && (
                <GraphCreationForm
                    onCreate={createGraph}
                    onClose={() => setIsCreateOpen(false)}
                />
            )}

            {
                project.graphs.map (
                    graph => (
                        <GraphEntry
                            key={graph.id}
                            projectId={project.id}
                            id={graph.id}
                            name={graph.name}
                            createdAt={graph.createdAt}
                            updatedAt={graph.updatedAt}
                            projectName={project.name}
                        />
                    )
                )
            }
        </div>
    );
}

export default ProjectPage;
