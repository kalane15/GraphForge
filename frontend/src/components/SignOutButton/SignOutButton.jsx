import { useNavigate } from "react-router";


function SignOutButton() {
    const navigate = useNavigate();
    async function signOut() {
        await fetch(`${import.meta.env.VITE_BACKEND_URL}/auth/signout`, {
            method: "POST",
            credentials: "include"
        });
        navigate(`/`);
    }

    return (
        <div className="centered-container border-container header-button">
            <button onClick={signOut}>
                Sign Out
            </button>
        </div>
    );
}

export default SignOutButton;