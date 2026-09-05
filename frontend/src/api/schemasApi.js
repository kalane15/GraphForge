import { request } from "./request"


export async function getSchemasRequest(projectId) {
    const path = `/projects/${projectId}/schemas`;

    return await request(path);
}
