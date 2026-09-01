import "@/styles/common.css";
import { useNavigate } from "react-router";
import { useState } from "react";
import ProjectForm from "./ProjectForm";
import ConfirmForm from "@/components/ConfirmForm/ConfirmForm"


function ProjectEntry(props) {
    const [ isUpdateOpen, setIsUpdateOpen ] = useState(false);
    const [ isConfirmOpen, setIsConfirmOpen ] = useState(false);

    const navigate = useNavigate();

    async function loadProject() {
        navigate(`/projects/${props.id}`);
    }

    async function updateProject(name, description) {
        await props.onUpdate(props.id, name, description);        
    }

    async function deleteProject() {
        await props.onDelete(props.id);
    }

    return (
        <>
            <div className="list-entry list-entry--project">
                <div className="list-entry__field">
                    <span className="list-entry__label">Name</span>
                    <span className="list-entry__value">{props.name}</span>
                </div>

                <div className="list-entry__field">
                    <span className="list-entry__label">Id</span>
                    <span className="list-entry__value list-entry__value--mono">{props.id}</span>
                </div>


                <div className="list-entry__field">
                    <span className="list-entry__label">Graph count</span>
                    <span className="list-entry__value list-entry__value--mono">{props.graphCount}</span>
                </div>

                <div className="list-entry__field">
                    <span className="list-entry__label">Description</span>
                    <span className="list-entry__value">{props.description ?? "-"}</span>
                </div>

                <div className="list-entry__actions">
                    <button className="list-entry__button" onClick={loadProject}>
                        Load
                    </button>

                    <button className="list-entry__button" onClick={ () => setIsUpdateOpen(true) }>
                        Update
                    </button>

                    <button className="list-entry__button" onClick={() => setIsConfirmOpen(true)}>
                        Delete
                    </button>
                </div>
            </div>

            {isUpdateOpen && (
                <ProjectForm
                    onSubmit={updateProject}
                    onClose={() => setIsUpdateOpen(false)}
                    name={props.name}
                    description={props.description}
                    title="Update project"
                    submitText="Update"
                />
            )}

            {isConfirmOpen && (
                <ConfirmForm
                    onConfirm={deleteProject}
                    onClose={() => setIsConfirmOpen(false)}
                />
            )}
        </>
    );
}

export default ProjectEntry;
