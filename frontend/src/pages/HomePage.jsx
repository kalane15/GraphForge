import MainPageDescription from "@/components/GFDescription/GFDescription";
import SignInButton from "@/components/SignInButton/SignInButton";
import MainPageImage from "@/components/MainPageImage/MainPageImage";

function HomePage() {
    return (
        <div id="main" className="border-container">
            <div id="left-col" className="border-container">
                <MainPageDescription />
                <SignInButton />
            </div>

            <MainPageImage />
        </div>
    );
}

export default HomePage;