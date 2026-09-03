import { request } from "./request"


export async function signOutRequest() {
    const path = `/auth/signout`;
    const details =  {
        method: "POST",
    };

    await request(path, details);
}

export async function signInRequest(login, password) {
    const path = `/auth/signin`;
    const details = {
        method: "POST",

        headers: {
            "Content-Type": "application/json"
        },

        body: JSON.stringify({
            login: login,
            password: password
        })
    };

    await request(path, details);
}

export async function signUpRequest(login, password) {
    const path = `/auth/signup`
    const details = {
        method: "POST",

        headers: {
            "Content-Type": "application/json"
        },

        body: JSON.stringify({
            login: login,
            password: password
        })
    }

    await request(path, details);
}

export async function meRequest() {
    const path = "/auth/me";
    return await request(path, { });
}

export async function refreshRequest() {
    return await request("/auth/refresh", {
        method: "POST"
    });
}
