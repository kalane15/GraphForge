import { useEffect, useState } from "react";
import { Navigate, useLocation } from "react-router";

function ProtectedRoute({ children }) {
    const [isAuthenticated, setIsAuthenticated] = useState(null);
    const location = useLocation();

    useEffect(() => {
        async function checkAuth() {
            const response = await fetch(
                `${import.meta.env.VITE_BACKEND_URL}/auth/me`,
                {
                    credentials: "include"
                }
            );

            setIsAuthenticated(response.ok);
        }

        checkAuth();
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
