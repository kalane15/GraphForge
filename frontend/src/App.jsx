import { Routes, Route } from "react-router";
import Header from "@/components/Header/Header";
import HomePage from "@/pages/HomePage/HomePage";


function App() {
    return (
        <>
            <Header />
            <Routes>
                <Route path="/" element={<HomePage />} />
            </Routes>
        </>
    );
}

export default App;