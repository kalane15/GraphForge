function SignInButton() {
    function handleClick() {
        console.log("Button clicked");
    }

    return <button onClick={handleClick}>
        Sign In
    </button>
}

export default SignInButton;