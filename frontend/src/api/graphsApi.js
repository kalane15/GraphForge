import { request } from "./request"

export async function getGraphRequest(graphId) {
    
}

export async function createGraphRequest(name, projectId) {
    const path = `/graphs`;
    const details = {
        method: "POST",

        headers: {
            "Content-Type": "application/json"
        },

        body: JSON.stringify({ name, projectId })
    };

    return await request(path, details);
}
