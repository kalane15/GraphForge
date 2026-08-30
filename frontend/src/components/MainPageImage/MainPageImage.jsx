import image from "@/assets/main_page_image.png";
import "@/styles/common.css";

function MainPageImage() {
    return (
        <div id="image-container" className="centered-container border-container">
            <img className="image-contain" src={image}></img>
        </div>
    );
}
export default MainPageImage;