import { useEffect, useState } from "react";
import { useLocation, useNavigate } from "react-router";
import "@/styles/common.css";


function ProjectsPage() {
    const [projects, setProjects] = useState([]);
    const [message, setMessage] = useState("");
    const navigate = useNavigate();
    const location = useLocation();

    async function loadProjects() {
        const response = await fetch(
            `${import.meta.env.VITE_BACKEND_URL}/projects`,
            {
                credentials: "include"
            }
        );

        if (response.status === 401) {
            navigate("/auth", { state: { from: location }, replace: true });
            return [];
        }

        if (!response.ok) {
            throw new Error("Failed to load projects");
        }

        const data = await response.json();
        return data;
    }

    useEffect(() => {
        async function fetchProjects() {
            try {
                const data = await loadProjects();
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
