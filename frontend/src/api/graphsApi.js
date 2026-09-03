import { request } from "./request"

export async function getGraphRequest(graphId) {
    
}

export async function createGraphRequest(name, projectId) {
    const path = `/projects/${projectId}/graphs`;
    const details = {
        method: "POST",

        headers: {
            "Content-Type": "application/json"
        },

        body: JSON.stringify({ name, projectId })
    };

    return await request(path, details);
}

export async function deleteGraphRequest(graphId, projectId) {
    const path = `/projects/${projectId}/graphs/${graphId}`;
    const details = {
        method: "DELETE"
    };

    return await request(path, details);
}
