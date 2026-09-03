import { Routes, Route } from "react-router";
import Header from "@/components/Header/Header";
import HomePage from "@/pages/HomePage/HomePage";
import AuthPage from "@/pages/AuthPage/AuthPage";
import ProjectsListPage from "@/pages/ProjectsListPage/ProjectsListPage";
import ProjectPage from "@/pages/ProjectPage/ProjectPage";
import ProtectedRoute from "@/components/ProtectedRoute/ProtectedRoute";
import GraphEditorPage from "@/pages/GraphEditorPage/GraphEditorPage"


function App() {
    return (
        <>
            <Header />
            <Routes>
                <Route path="/" element={<HomePage />} />
                <Route path="/auth" element={<AuthPage />} />

                <Route path="/projects" element={
                    <ProtectedRoute>
                        <ProjectsListPage />
                    </ProtectedRoute>
                } />

                <Route path="/projects/:projectId" element={
                    <ProtectedRoute>
                        <ProjectPage />
                    </ProtectedRoute>
                } />

                <Route path="/projects/:projectId/graphs/:graphId/edit" element={
                    <ProtectedRoute>
                        <GraphEditorPage />
                    </ProtectedRoute>
                } />

            </Routes>
        </>
    );
}

export default App;
