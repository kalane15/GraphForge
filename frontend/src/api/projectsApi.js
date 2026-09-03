import { request, API_URL } from "./request"


export async function getProjectsListRequest() {
    const path = `${API_URL}/projects`;

    await request(path, {});
}

export async function getProjectRequest() {

}

export async function updateProjectMetadataRequest() {

}

export async function deleteProjectRequest() {

}