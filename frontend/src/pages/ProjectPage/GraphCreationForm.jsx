import "@/styles/common.css";

import { useState } from "react";

function GraphCreationForm({ onCreate, onClose }) {
    const [name, setName] = useState("");

    async function handleSubmit(event) {
        event.preventDefault();

        await onCreate(
            name
        );

        onClose();
    }

    return (
        <div className="modal-overlay">
            <form className="modal" onSubmit={handleSubmit}>
                <h2>Create project</h2>

                <input
                    placeholder="Graph name"
                    value={name}
                    onChange={(event) => setName(event.target.value)}
                />

                <button type="button" onClick={onClose}>
                    Cancel
                </button>

                <button type="submit">
                    Create
                </button>
                
            </form>
        </div>
    );
}

export default GraphCreationForm;
