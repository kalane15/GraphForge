import { useNavigate } from "react-router";
import { signOutRequest } from "@/api/authApi";

function SignOutButton() {
    const navigate = useNavigate();
    async function signOut() {
        try {
            await signOutRequest();            
        } catch (error) {
            console.error("Error signing out:", error.message);
        } finally {
            navigate(`/`);
        } 
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