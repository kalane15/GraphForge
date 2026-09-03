const API_URL = import.meta.env.VITE_BACKEND_URL;


export async function request(path, options = {}) {
    const response = await fetch(`${API_URL}${path}`, {
        credentials: "include",
        ...options,
        headers: {
            ...options.headers
        }
    });

    if (!response.ok) {
        const data = await response.json();
        const message = data.detail;

        throw new Error(message);
    }

    if (response.status === 204) {
        return null;
    }

    const text = await response.text();

    if (!text) {
        return null;
    }

    return JSON.parse(text);
}