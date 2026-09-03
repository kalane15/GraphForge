import { request, API_URL } from "./request"


export async function getProjectsListRequest() {
    const path = `${API_URL}/projects`;

    return await request(path, {});
}

export async function getProjectRequest(projectId) {
    const path = `${API_URL}/projects/${projectId}`;

    return await request(path, {});
}

export async function updateProjectMetadataRequest() {

}

export async function createProjectRequest(name, description) {
    const path = `${API_URL}/projects`;
    const details = {
        method: "POST",
        credentials: "include",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ name, description })
    };

    return await request(path, details);
}

export async function deleteProjectRequest(id) {
    const path = `${API_URL}/projects/${id}`;
    return await request(path, { method: "DELETE" });
}
