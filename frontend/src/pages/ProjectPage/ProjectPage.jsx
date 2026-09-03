import { useEffect, useState } from "react";
import { useParams } from "react-router";
import GraphCreationForm from "./GraphCreationForm";
import GraphEntry from "./GraphEntry";
import { getProjectRequest } from "@/api/projectsApi";
import { createGraphRequest } from "@/api/graphsApi";
import { deleteGraphRequest } from "@/api/graphsApi";


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

                const data = await getProjectRequest(projectId);
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
    }, [projectId]);

    async function createGraph(name) {
        try {
            const graph = await createGraphRequest(name, projectId);
            setProject({ ...project, graphs: [...project.graphs, graph] });
        } catch (error) {
            setError(error.message);
        }        
    }

    async function deleteGraph(graphId) {
        try {
            await deleteGraphRequest(graphId, projectId);
            setProject({ ...project, graphs: project.graphs.filter(g => g.id !== graphId) });
        } catch (error) {
            setError(error.message);
        }
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
                            onDelete={deleteGraph}
                        />
                    )
                )
            }
        </div>
    );
}

export default ProjectPage;
