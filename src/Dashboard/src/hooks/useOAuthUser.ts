import { useUser } from '@/context/UserContext';
import { useApi } from '@/hooks/useApi';

export function useOAuthUser() {
  const { userProfile, isAuthenticated, isLoading, setUserProfile, refreshUserProfile } = useUser();
  const { baseUrl } = useApi();

  const checkOAuthStatus = async (businessId: string) => {
    try {
      const response = await fetch(`${baseUrl}/oauth/google/status`, {
        headers: {
          'Content-Type': 'application/json',
          'X-Business-Id': businessId,
        },
      });

      if (response.ok) {
        const data = await response.json();
        if (data.isConnected && data.userProfile) {
          setUserProfile(data.userProfile);
          return { success: true, profile: data.userProfile };
        } else {
          setUserProfile(null);
          return { success: false, profile: null };
        }
      } else {
        setUserProfile(null);
        return { success: false, profile: null };
      }
    } catch (error) {
      console.error('Failed to check OAuth status:', error);
      setUserProfile(null);
      return { success: false, profile: null };
    }
  };

  const handleOAuthSuccess = async (businessId: string) => {
    // Refresh user profile after successful OAuth
    await checkOAuthStatus(businessId);
  };

  const signOut = () => {
    setUserProfile(null);
  };

  return {
    userProfile,
    isAuthenticated,
    isLoading,
    checkOAuthStatus,
    handleOAuthSuccess,
    refreshUserProfile,
    signOut,
  };
}