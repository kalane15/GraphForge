import { useState } from "react";


function SignInForm() {
    const [login, setLogin] = useState("");
    const [password, setPassword] = useState("");
    const [resultInfo, setResultInfo] = useState("");

    async function handleSubmit(event) {
        event.preventDefault();

        const response = await fetch(`${import.meta.env.VITE_BACKEND_URL}/auth/signin`, {
            method: "POST",

            headers: {
                "Content-Type": "application/json"
            },

            body: JSON.stringify({
                login: login,
                password: password
            })
        });
        const data = await response.json();

        if (response.ok) {
            setResultInfo("Wellcome");
        } else {
            setResultInfo(data.message);
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
            <label>{ resultInfo }</label><br />
        </div>
    );
}

export default SignInForm;