import { useState } from "react";
import "@/styles/common.css";
import "./AuthPage.css";
import SignUpForm from "./SignUpForm";
import SignInForm from "./SignInForm";


function AuthPage() {
    const [isSignInForm, setIsSignInForm] = useState(true);

    async function handleSubmit(event) {
        event.preventDefault();

        const response = await fetch(`${import.meta.env.VITE_BACKEND_URL}/auth/register`, {
            method: "POST",

            headers: {
                "Content-Type": "application/json"
            },

            body: JSON.stringify({
                login: login,
                password: password
            })
        });

        if (response.ok) {
            console.log("Успешный вход");
        } else {
            console.log("Ошибка входа");
        }
    }

    return (
        <div id="auth" className="centered-container">
            <div>
                {isSignInForm ? <SignInForm /> : <SignUpForm />}
            </div>
            <div>
                <button onClick={() => setIsSignInForm(!isSignInForm)}>Change to {isSignInForm ? "sign up" : "sign in"}</button>
            </div>
        </div>
    );
}

export default AuthPage;