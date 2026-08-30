import React from "react";
import ReactDOM from "react-dom/client";
import MainPageImage from "./src/components/MainPageImage/MainPageImage"
import MainPageDescription from "./src/components/GFDescription/GFDescription"
import SignInButton from "./src/components/SignInButton/SignInButton"
import { Component } from "react";


let rootNode = document.body;
let root = ReactDOM.createRoot(rootNode);
root.render(
    <>
        <div id="header" className="border-container"></div>
        <div id="main" className="border-container">
            <div id="left-col" className="border-container">
                <MainPageDescription />
                <SignInButton />
            </div>

            <MainPageImage />
        </div>
    </>
);

