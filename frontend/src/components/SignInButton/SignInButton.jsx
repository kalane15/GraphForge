import "@/styles/common.css";
import { useNavigate } from "react-router";

function SignInButton() {
    const navigate = useNavigate();

    function handleClick() {
        navigate("/auth");
    }

    return (
        <div id="sign-in" className="centered-container border-container">
            <button onClick={handleClick}>
                Sign In
            </button>
        </div>
    );
}

export default SignInButton;