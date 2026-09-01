import "./Header.css"
import HeaderButton from "@/components/HeaderButton/HeaderButton"
import SignOutButton from "@/components/SignOutButton/SignOutButton"


function Header() {
    return (
        <div id="header" className="header">
            <HeaderButton title="MainPage" path="/"/>
            <HeaderButton title="Projects" path="/projects" />
            <SignOutButton />
        </div>
    );
}

export default Header;