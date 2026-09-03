import { request, API_URL } from "./request"


export async function signOutRequest() {
    const path = `${API_URL}/auth/signout`;
    const details =  {
        method: "POST",
    };

    await request(path, details);
}

export async function signInRequest(login, password) {
    const path = `${API_URL}/auth/signin`;
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
    const path = `${API_URL}/auth/signup`
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