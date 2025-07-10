import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';

interface UserProfile {
  name: string;
  email: string;
  picture: string;
  givenName: string;
  familyName: string;
}

interface UserContextType {
  userProfile: UserProfile | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  setUserProfile: (profile: UserProfile | null) => void;
  refreshUserProfile: () => Promise<void>;
}

const UserContext = createContext<UserContextType | undefined>(undefined);

interface UserProviderProps {
  children: ReactNode;
}

export function UserProvider({ children }: UserProviderProps) {
  const [userProfile, setUserProfile] = useState<UserProfile | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  const isAuthenticated = userProfile !== null;

  const refreshUserProfile = async () => {
    try {
      setIsLoading(true);
      
      // Get business ID from wherever it's stored (for now using 1)
      const businessId = "1";
      
      const response = await fetch(`https://localhost:7157/api/oauth/google/status`, {
        headers: {
          'Content-Type': 'application/json',
          'X-Business-Id': businessId,
        },
      });

      if (response.ok) {
        const data = await response.json();
        if (data.isConnected && data.userProfile) {
          setUserProfile(data.userProfile);
        } else {
          setUserProfile(null);
        }
      } else {
        setUserProfile(null);
      }
    } catch (error) {
      console.error('Failed to refresh user profile:', error);
      setUserProfile(null);
    } finally {
      setIsLoading(false);
    }
  };

  // Load user profile on mount
  useEffect(() => {
    refreshUserProfile();
  }, []);

  return (
    <UserContext.Provider
      value={{
        userProfile,
        isAuthenticated,
        isLoading,
        setUserProfile,
        refreshUserProfile,
      }}
    >
      {children}
    </UserContext.Provider>
  );
}

export function useUser() {
  const context = useContext(UserContext);
  if (context === undefined) {
    throw new Error('useUser must be used within a UserProvider');
  }
  return context;
}