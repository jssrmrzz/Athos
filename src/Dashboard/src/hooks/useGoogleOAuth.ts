import { useState, useEffect, useCallback } from 'react';

interface OAuthStatus {
  isConnected: boolean;
  hasToken: boolean;
  expiresAt?: string;
  scope?: string;
  isExpired?: boolean;
  isRevoked?: boolean;
}

interface UseGoogleOAuthProps {
  businessId: string;
  autoRefresh?: boolean;
}

export const useGoogleOAuth = ({ businessId, autoRefresh = true }: UseGoogleOAuthProps) => {
  const [status, setStatus] = useState<OAuthStatus>({ isConnected: false, hasToken: false });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const checkStatus = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      
      const response = await fetch(`/api/oauth/google/status`, {
        headers: {
          'Content-Type': 'application/json',
          'X-Business-Id': businessId,
        },
      });

      if (response.ok) {
        const data = await response.json();
        setStatus(data);
        
        // Auto-refresh if token is expired and auto-refresh is enabled
        if (autoRefresh && data.hasToken && data.isExpired && !data.isRevoked) {
          await refreshToken();
        }
      } else {
        throw new Error('Failed to check OAuth status');
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to check OAuth status');
    } finally {
      setLoading(false);
    }
  }, [businessId, autoRefresh]);

  const authorize = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      
      const response = await fetch(`/api/oauth/google/authorize`, {
        headers: {
          'Content-Type': 'application/json',
          'X-Business-Id': businessId,
        },
      });

      if (response.ok) {
        const data = await response.json();
        window.location.href = data.authorizationUrl;
      } else {
        throw new Error('Failed to generate authorization URL');
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to start OAuth flow');
      setLoading(false);
    }
  }, [businessId]);

  const refreshToken = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      
      const response = await fetch(`/api/oauth/google/refresh`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'X-Business-Id': businessId,
        },
      });

      if (response.ok) {
        await checkStatus();
      } else {
        throw new Error('Failed to refresh token');
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to refresh token');
    } finally {
      setLoading(false);
    }
  }, [businessId, checkStatus]);

  const revokeToken = useCallback(async () => {
    try {
      setLoading(true);
      setError(null);
      
      const response = await fetch(`/api/oauth/google/revoke`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'X-Business-Id': businessId,
        },
      });

      if (response.ok) {
        await checkStatus();
      } else {
        throw new Error('Failed to revoke token');
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to revoke token');
    } finally {
      setLoading(false);
    }
  }, [businessId, checkStatus]);

  useEffect(() => {
    if (businessId) {
      checkStatus();
    }
  }, [businessId, checkStatus]);

  return {
    status,
    loading,
    error,
    authorize,
    refreshToken,
    revokeToken,
    checkStatus,
  };
};