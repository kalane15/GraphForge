import { useEffect, useState } from "react";
import { Navigate, useLocation } from "react-router";
import { meRequest, refreshRequest } from "@/api/authApi";

function ProtectedRoute({ children }) {
    const [isAuthenticated, setIsAuthenticated] = useState(null);
    const location = useLocation();

    useEffect(() => {
        async function checkAuth() {
            try {
                await meRequest();
                return true;
            } catch (error) {
                return false;
            }
        }

        async function tryAuth() {
            if (await checkAuth()) {
                setIsAuthenticated(true);
                return;
            }

            try {
                await refreshRequest();
            } catch (error) {
                setIsAuthenticated(false);
                return;
            }

            if (await checkAuth()) {
                setIsAuthenticated(true);
                return;
            }

            setIsAuthenticated(false);                    
        }

        tryAuth();
    }, []);

    if (isAuthenticated === null) {
        return <div>Loading...</div>;
    }

    if (!isAuthenticated) {
        return (
            <Navigate
                to="/auth"
                state={{ from: location }}
                replace
            />
        );
    }

    return children;
}

export default ProtectedRoute;
