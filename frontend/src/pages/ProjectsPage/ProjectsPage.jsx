import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router";
import { getProjectsListRequest } from "@/api/projectsApi";
import "@/styles/common.css";


function ProjectsPage() {
    const [projects, setProjects] = useState([]);
    const [message, setMessage] = useState("");
    const navigate = useNavigate();
    const location = useLocation();

    useEffect(() => {
        async function fetchProjects() {
            try {
                const data = await getProjectsListRequest();
                setProjects(data.projects);
            }
            catch (error) {
                setMessage(error.message);
            }
        }

        fetchProjects();
    }, []);

    return (
        <div>
            <h1>Projects</h1>

            {message && <p>{message}</p>}

            {projects.map(project => (
                <div key={project.id}>
                    {project.name}
                </div>
            ))}
        </div>
    );
}

export default ProjectsPage;
