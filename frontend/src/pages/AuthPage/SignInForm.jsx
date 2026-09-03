import { useState } from "react";
import { useNavigate } from "react-router";
import { signInRequest } from "@/api/authApi";

function SignInForm({ from = "/" }) {
    const [login, setLogin] = useState("");
    const [password, setPassword] = useState("");
    const [message, setMessage] = useState("");
    const navigate = useNavigate();

    async function handleSubmit(event) {
        event.preventDefault();

        try {
            await signInRequest(login, password);
            navigate(from, { replace: true });
        } catch (error) {
            setMessage(error.message);
        }
    }

    return (
        <div id="auth" className="centered-container">
            <label id="login-label">Sign In</label><br />
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
                    <label htmlFor="password" id="password-label" >Password</label><br />
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

export default SignInForm;
