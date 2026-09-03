import "@/styles/common.css";

import { useState } from "react";

function ProjectForm(props) {
    const [message, setMessage] = useState("");

    const [name, setName] = useState(props.name ?? "");
    const [description, setDescription] = useState(props.description ?? "");

    async function handleSubmit(event) {
        event.preventDefault();

        if (name === "") {
            setMessage("Project name cannot be empty");
            return;
        }

        await props.onSubmit(
            name,
            description
        );

        props.onClose();
    }

    return (
        <div className="modal-overlay">
            <form className="modal" onSubmit={handleSubmit}>
                <h2>{ props.title ?? "Edit project" }</h2>
                <label>{message}</label>

                <input
                    placeholder="Project name"
                    value={name}
                    onChange={(event) => setName(event.target.value)}
                />

                <input
                    placeholder="Project description"
                    value={description}
                    onChange={(event) => setDescription(event.target.value)}
                />


                <button type="button" onClick={props.onClose}>
                    Cancel
                </button>

                <button type="submit">
                    {props.submitText ?? "Submit"}
                </button>
            </form>
        </div>
    );
}

export default ProjectForm;
