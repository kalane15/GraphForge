import { request } from "./request"


export async function getProjectsListRequest() {
    const path = `/projects`;

    return await request(path, {});
}

export async function getProjectRequest(projectId) {
    const path = `/projects/${projectId}`;

    return await request(path, {});
}

export async function updateProjectMetadataRequest(id, name, description) {
    const path = `/projects/${id}`;
    const details = {
        method: "PUT",

        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify({ name, description })
    };

    return await request(path, details);
}

export async function createProjectRequest(name, description) {
    const path = `/projects`;
    const details = {
        method: "POST",

        headers: {
            "Content-Type": "application/json"
        },

        body: JSON.stringify({ name, description })
    };

    return await request(path, details);
}

export async function deleteProjectRequest(id) {
    const path = `/projects/${id}`;
    return await request(path, { method: "DELETE" });
}
