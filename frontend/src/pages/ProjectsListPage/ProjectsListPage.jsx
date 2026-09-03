import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router";
import "@/styles/common.css";
import ProjectForm from "./ProjectForm";
import ProjectEntry from "./ProjectEntry";
import { getProjectsListRequest, deleteProjectRequest, createProjectRequest } from "@/api/projectsApi";


function ProjectsListPage() {
    const [projects, setProjects] = useState([]);
    const [message, setMessage] = useState("");

    const [isCreateOpen, setIsCreateOpen] = useState(false);

    const navigate = useNavigate();

    const location = useLocation();

    async function updateProject(id, name, description) {
        const response = await fetch(
            `${import.meta.env.VITE_BACKEND_URL}/projects/${id}`,
            {
                method: "PUT",
                credentials: "include",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({ name, description })
            }
        );

        if (!response.ok) {
            const text = await response.text();
            const message = text ? JSON.parse(text).message : response.statusText;
            throw new Error(message);
        }

        const updatedProject = await response.json();

        setProjects(prevProjects =>
            prevProjects.map(project =>
                project.id === id
                    ? {
                        ...project,
                        name: updatedProject.name,
                        description: updatedProject.description
                    }
                    : project
            )
        );
    }


    async function createProject(name, description) {
        try {
            const data = await createProjectRequest(name, description);
            setProjects((prevProjects) => [...prevProjects, data]);
        } catch (error) {
            setMessage(error.message);
        }        
    }


    async function deleteProject(id) {
        try {
            await deleteProjectRequest(id);
        }
        catch (error) {
            setMessage(error.message);
        }
    }

    useEffect(() => {
        async function fetchProjects() {
            try {
                const data = await getProjectsListRequest();
            } catch (error) {
                setMessage(error.message);
            }
        }

        setProjects((prevProjects) => prevProjects.filter(project => project.id !== id));
    }, []);


    return (
        <div>
            <h1>Projects</h1>

            {message && <p>{message}</p>}

            <button onClick={() => setIsCreateOpen(true)}>
                Create project
            </button>

            {isCreateOpen && (
                <ProjectForm
                    onSubmit={createProject}
                    onClose={() => setIsCreateOpen(false)}
                    title="Create project"
                    submitText="Create"
                />
            )}

            {projects.map(project => (
                <ProjectEntry
                    key={project.id}
                    id={project.id}
                    name={project.name}
                    description={project.description}
                    graphCount={project.graphCount}
                    onUpdate={updateProject}
                    onDelete={deleteProject}
                />
            ))}
        </div>
    );
}

export default ProjectsListPage;
