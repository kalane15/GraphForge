import "@/styles/common.css";
function SignInButton() {
    function handleClick() {
        console.log("Button clicked");
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