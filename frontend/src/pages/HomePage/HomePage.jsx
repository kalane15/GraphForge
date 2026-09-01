import GFDescription from "@/components/GFDescription/GFDescription";
import SignInButton from "@/components/SignInButton/SignInButton";
import HomePageImage from "./HomePageImage/HomePageImage";
import "./HomePage.css";

function HomePage() {
    return (
        <div id="main" className="border-container">
            <div id="left-col" className="border-container">
                <GFDescription />
                <SignInButton />
            </div>

            <HomePageImage />
        </div>
    );
}

export default HomePage;