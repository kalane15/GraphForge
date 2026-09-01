import { useState } from "react";
import { useNavigate } from "react-router";


function RegisterForm({ from = "/" }) {
    const [login, setLogin] = useState("");
    const [password, setPassword] = useState("");
    const [message, setMessage] = useState("");
    const navigate = useNavigate();

    async function handleSubmit(event) {
        event.preventDefault();

        const response = await fetch(`${import.meta.env.VITE_BACKEND_URL}/auth/signup`, {
            method: "POST",
            credentials: "include",

            headers: {
                "Content-Type": "application/json"
            },

            body: JSON.stringify({
                login: login,
                password: password
            })
        });

        setMessage(await response.json().then(data => data.message));

        if (response.ok) {
            navigate(from, { replace: true });
        }
    }

    return (
        <div id="auth" className="centered-container">
            <label id="login-label">Sign Up</label><br />
            <form onSubmit={handleSubmit}>
                <p>
                    <label htmlFor="login" id="login-label">Login</label><br />
                    <input
                        id="login"
                        type="text"
                        value={login}
                        onChange={(event) => setLogin(event.target.value)}
                    />
                </p>

                <p>
                    <label htmlFor="password" id="password-label">Password</label><br />
                    <input
                        id="password"
                        type="password"
                        value={password}
                        onChange={(event) => setPassword(event.target.value)}
                    />
                </p>

                <input id="submit-btn" type="submit" value="Submit" />
            </form>
            <label>{ message }</label><br />
        </div>
    );
}

export default RegisterForm;
