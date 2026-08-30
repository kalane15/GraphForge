import React from "react";
import ReactDOM from "react-dom/client";
import MainMenuImage from "./src/components/MainPageImage"
import MainPageDescription from "./src/components/MainPageDescription"
import SignInButton from "./src/components/SignInButton"
import { Component } from "react";

class ClickButton extends Component {
    constructor(props) {
        super(props);
        this.press = this.press.bind(this);
    }
    press(e) {
        console.log(e); // выводим информацию о событии
        console.log("Hello METANIT.COM!");
    }
    render() {
        return <button onClick={this.press}>Click</button>;
    }
}

let rootNode = document.getElementById("image-container");
let root = ReactDOM.createRoot(rootNode);
root.render(
    <MainMenuImage />
);

rootNode = document.getElementById("description");
root = ReactDOM.createRoot(rootNode);
root.render(
    <MainPageDescription />
);

rootNode = document.getElementById("sign-in");
root = ReactDOM.createRoot(rootNode);
root.render(
    <SignInButton />
);

