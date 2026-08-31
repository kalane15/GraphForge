import { Routes, Route } from "react-router";
import Header from "@/components/Header/Header";
import HomePage from "@/pages/HomePage/HomePage";
import AuthPage from "@/pages/AuthPage/AuthPage";

function App() {
    return (
        <>
            <Header />
            <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/auth" element={<AuthPage />} />
            </Routes>
        </>
    );
}

export default App;