import { useState } from "react";
import { useLocation } from "react-router";
import "@/styles/common.css";
import "./AuthPage.css";
import SignUpForm from "./SignUpForm";
import SignInForm from "./SignInForm";


function AuthPage() {
    const [isSignInForm, setIsSignInForm] = useState(true);
    const location = useLocation();
    const fromLocation = location.state?.from;
    const from = fromLocation
        ? `${fromLocation.pathname}${fromLocation.search}${fromLocation.hash}`
        : "/";

    return (
        <div id="auth" className="centered-container">
            <div>
                {isSignInForm ? <SignInForm from={ from } /> : <SignUpForm from={ from } />}
            </div>
            <div>
                <button onClick={ () => setIsSignInForm(!isSignInForm) }>Change to { isSignInForm ? "sign up" : "sign in" }</button>
            </div>
        </div>
    );
}

export default AuthPage;
