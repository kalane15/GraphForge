import { useNavigate } from "react-router-dom";
function HeaderButton(props) {
    const navigate = useNavigate;
    function moveToPage() {
        navigate(`/${props.path}`);
    }
    return (
        <div id="sign-in" className="centered-container border-container">
            <button onClick={moveToPage}>
                {props.title}
            </button>
        </div>
    );
}

export default HeaderButton;