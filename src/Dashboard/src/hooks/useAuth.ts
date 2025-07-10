import { useNavigate } from "react-router-dom";
import { useApi } from "./useApi";
import { useOAuthUser } from "./useOAuthUser";

export function useAuth() {
    const navigate = useNavigate();
    const { baseUrl } = useApi();
    const { signOut: clearUserProfile } = useOAuthUser();

    const signOut = async () => {
        try {
            // Get current business ID (hardcoded for now, should come from context in future)
            const businessId = "1";
            
            // Call the OAuth revoke endpoint (same as Disconnect feature)
            const response = await fetch(`${baseUrl}/oauth/google/revoke`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'X-Business-Id': businessId,
                },
            });

            if (response.ok) {
                console.log('Successfully signed out and disconnected from Google');
                
                // Clear user profile from context (same as Disconnect does)
                clearUserProfile();
                
                // Clear any local storage or session storage
                localStorage.clear();
                sessionStorage.clear();
                
                // Navigate to home page
                navigate('/', { replace: true });
                
                return { success: true };
            } else {
                console.error('Sign out failed');
                
                // Still clear local data and redirect even if server revoke failed
                clearUserProfile();
                localStorage.clear();
                sessionStorage.clear();
                navigate('/', { replace: true });
                
                return { success: false, error: 'Server error during sign out' };
            }
        } catch (error) {
            console.error('Sign out error:', error);
            
            // Even if the API call fails, clear local data and redirect
            clearUserProfile();
            localStorage.clear();
            sessionStorage.clear();
            navigate('/', { replace: true });
            
            return { success: false, error: 'Network error during sign out' };
        }
    };

    return {
        signOut
    };
}