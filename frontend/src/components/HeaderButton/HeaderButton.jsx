import { useNavigate } from "react-router";
import "./HeaderButton.css";


function HeaderButton(props) {
    const navigate = useNavigate();
    function moveToPage() {
        navigate(`${props.path}`);
    }

    return (
        <div id="sign-in" className="centered-container border-container header-button">
            <button onClick={moveToPage}>
                {props.title}
            </button>
        </div>
    );
}

export default HeaderButton;