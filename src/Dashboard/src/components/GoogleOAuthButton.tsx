import React, { useState, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { CheckCircle, XCircle, RefreshCw, AlertCircle } from 'lucide-react';
import { useApi } from '@/hooks/useApi';
import { useOAuthUser } from '@/hooks/useOAuthUser';
import { useToast } from '@/hooks/use-toast';

interface OAuthStatus {
  isConnected: boolean;
  hasToken: boolean;
  expiresAt?: string;
  scope?: string;
  isExpired?: boolean;
  isRevoked?: boolean;
}

interface GoogleOAuthButtonProps {
  businessId: string;
  onStatusChange?: (status: OAuthStatus) => void;
}

export const GoogleOAuthButton: React.FC<GoogleOAuthButtonProps> = ({ businessId, onStatusChange }) => {
  const [status, setStatus] = useState<OAuthStatus>({ isConnected: false, hasToken: false });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const { baseUrl } = useApi();
  const { checkOAuthStatus, handleOAuthSuccess, signOut } = useOAuthUser();
  const { toast } = useToast();

  // Check OAuth status on mount and when businessId changes
  useEffect(() => {
    checkOAuthStatusInternal();
  }, [businessId]);

  // Check for OAuth success from URL params (after redirect)
  useEffect(() => {
    const urlParams = new URLSearchParams(window.location.search);
    const oauthSuccess = urlParams.get('oauth');
    
    if (oauthSuccess === 'success') {
      toast({
        title: "Authentication Successful!",
        description: "You have been successfully connected to Google.",
        duration: 5000,
      });
      
      // Remove the oauth parameter from URL
      window.history.replaceState({}, '', window.location.pathname);
      
      // Refresh status and user profile
      handleOAuthSuccess(businessId);
      checkOAuthStatusInternal();
    } else if (oauthSuccess === 'error') {
      toast({
        title: "Authentication Failed",
        description: "There was an error connecting to Google. Please try again.",
        variant: "destructive",
        duration: 5000,
      });
      
      // Remove the oauth parameter from URL
      window.history.replaceState({}, '', window.location.pathname);
    }
  }, []);

  const checkOAuthStatusInternal = async () => {
    try {
      setLoading(true);
      const result = await checkOAuthStatus(businessId);
      
      if (result.success) {
        setStatus({ 
          isConnected: true, 
          hasToken: true,
          // Add other properties as needed from the response 
        });
        onStatusChange?.({ 
          isConnected: true, 
          hasToken: true 
        });
      } else {
        setStatus({ isConnected: false, hasToken: false });
        onStatusChange?.({ isConnected: false, hasToken: false });
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to check OAuth status');
      setStatus({ isConnected: false, hasToken: false });
    } finally {
      setLoading(false);
    }
  };

  const handleAuthorize = () => {
    try {
      setLoading(true);
      setError(null);
      
      // Direct navigation to OAuth endpoint - server will handle redirect
      const authUrl = new URL(`${baseUrl}/oauth/google/authorize`, window.location.origin);
      authUrl.searchParams.set('businessId', businessId);
      
      window.location.href = authUrl.toString();
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to start OAuth flow');
      setLoading(false);
    }
  };

  const handleRefresh = async () => {
    try {
      setLoading(true);
      setError(null);
      
      const response = await fetch(`${baseUrl}/oauth/google/refresh`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'X-Business-Id': businessId,
        },
      });

      if (response.ok) {
        await checkOAuthStatusInternal();
        await handleOAuthSuccess(businessId);
        
        toast({
          title: "Token Refreshed",
          description: "Your authentication token has been refreshed successfully.",
          duration: 3000,
        });
      } else {
        throw new Error('Failed to refresh token');
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to refresh token');
      toast({
        title: "Refresh Failed",
        description: "Failed to refresh your authentication token. Please try again.",
        variant: "destructive",
        duration: 3000,
      });
    } finally {
      setLoading(false);
    }
  };

  const handleRevoke = async () => {
    try {
      setLoading(true);
      setError(null);
      
      const response = await fetch(`${baseUrl}/oauth/google/revoke`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'X-Business-Id': businessId,
        },
      });

      if (response.ok) {
        await checkOAuthStatusInternal();
        // Clear user profile from context
        signOut();
        
        toast({
          title: "Disconnected",
          description: "You have been successfully disconnected from Google.",
          duration: 3000,
        });
      } else {
        throw new Error('Failed to revoke token');
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to revoke token');
      toast({
        title: "Disconnect Failed",
        description: "Failed to disconnect from Google. Please try again.",
        variant: "destructive",
        duration: 3000,
      });
    } finally {
      setLoading(false);
    }
  };

  const getStatusIcon = () => {
    if (status.isConnected) {
      return <CheckCircle className="h-5 w-5 text-green-500" />;
    } else if (status.hasToken && status.isExpired) {
      return <AlertCircle className="h-5 w-5 text-yellow-500" />;
    } else if (status.hasToken && status.isRevoked) {
      return <XCircle className="h-5 w-5 text-red-500" />;
    } else {
      return <XCircle className="h-5 w-5 text-gray-400" />;
    }
  };

  const getStatusText = () => {
    if (status.isConnected) {
      return 'Connected to Google My Business';
    } else if (status.hasToken && status.isExpired) {
      return 'Token expired - Click refresh to reconnect';
    } else if (status.hasToken && status.isRevoked) {
      return 'Token revoked - Reconnect to continue';
    } else {
      return 'Not connected to Google My Business';
    }
  };

  return (
    <Card className="w-full max-w-md">
      <CardHeader>
        <CardTitle className="flex items-center gap-2">
          {getStatusIcon()}
          Google My Business Integration
        </CardTitle>
        <CardDescription>
          {getStatusText()}
        </CardDescription>
      </CardHeader>
      <CardContent className="space-y-4">
        {error && (
          <div className="text-red-500 text-sm bg-red-50 p-2 rounded">
            {error}
          </div>
        )}
        
        {status.isConnected && (
          <div className="text-sm text-gray-600 space-y-1">
            {status.expiresAt && (
              <p>Expires: {new Date(status.expiresAt).toLocaleString()}</p>
            )}
            {status.scope && (
              <p>Scopes: {status.scope}</p>
            )}
          </div>
        )}

        <div className="flex gap-2">
          {!status.isConnected && (
            <Button
              onClick={handleAuthorize}
              disabled={loading}
              className="flex-1"
            >
              {loading ? (
                <>
                  <RefreshCw className="h-4 w-4 mr-2 animate-spin" />
                  Connecting...
                </>
              ) : (
                'Connect Google My Business'
              )}
            </Button>
          )}

          {status.isConnected && (
            <>
              <Button
                onClick={handleRefresh}
                disabled={loading}
                variant="outline"
                size="sm"
              >
                {loading ? (
                  <RefreshCw className="h-4 w-4 animate-spin" />
                ) : (
                  <>
                    <RefreshCw className="h-4 w-4 mr-2" />
                    Refresh
                  </>
                )}
              </Button>
              <Button
                onClick={handleRevoke}
                disabled={loading}
                variant="destructive"
                size="sm"
              >
                Disconnect
              </Button>
            </>
          )}
        </div>
      </CardContent>
    </Card>
  );
};