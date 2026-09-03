import { useEffect, useState } from "react";
import "@/styles/common.css";
import ProjectForm from "./ProjectForm";
import ProjectEntry from "./ProjectEntry";

import {
    getProjectsListRequest,
    deleteProjectRequest,
    createProjectRequest,
    updateProjectMetadataRequest
} from "@/api/projectsApi";


function ProjectsListPage() {
    const [projects, setProjects] = useState([]);
    const [message, setMessage] = useState("");

    const [isCreateOpen, setIsCreateOpen] = useState(false);

    async function updateProject(id, name, description) {
        try {
            const updatedProject = await updateProjectMetadataRequest(id, name, description);

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
        } catch (error) {
            setMessage(error.message);
        }
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
            setProjects((prevProjects) => prevProjects.filter(project => project.id !== id));
        }
        catch (error) {
            setMessage(error.message);
        }
    }

    useEffect(() => {
        async function fetchProjects() {
            try {
                const data = await getProjectsListRequest();
                setProjects(data?.projects ?? []);
            } catch (error) {
                setMessage(error.message);
            }
        }

        fetchProjects();
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
