import { useNavigate } from "react-router-dom";

export function useAuth() {
    const navigate = useNavigate();

    const signOut = async () => {
        try {
            // Call the backend logout endpoint
            const response = await fetch('/api/auth/logout', {
                method: 'POST',
                credentials: 'include', // Include cookies
            });

            if (response.ok) {
                // Clear any local storage or session storage if needed
                localStorage.clear();
                sessionStorage.clear();
                
                // Redirect to login page or home page
                navigate('/login', { replace: true });
            } else {
                console.error('Sign out failed');
            }
        } catch (error) {
            console.error('Sign out error:', error);
            // Even if the API call fails, we can still clear local data and redirect
            localStorage.clear();
            sessionStorage.clear();
            navigate('/login', { replace: true });
        }
    };

    return {
        signOut
    };
}