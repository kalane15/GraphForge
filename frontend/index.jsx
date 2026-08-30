import React from "react";
import ReactDOM from "react-dom/client";
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

const rootNode = document.getElementById("app");
const root = ReactDOM.createRoot(rootNode);

root.render(
    <ClickButton />
);


