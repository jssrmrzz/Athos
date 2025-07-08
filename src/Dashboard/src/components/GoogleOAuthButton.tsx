import React, { useState, useEffect } from 'react';
import { Button } from '@/components/ui/button';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { CheckCircle, XCircle, RefreshCw, AlertCircle } from 'lucide-react';

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

  // Check OAuth status on mount
  useEffect(() => {
    checkOAuthStatus();
  }, [businessId]);

  const checkOAuthStatus = async () => {
    try {
      setLoading(true);
      const response = await fetch(`/api/oauth/google/status`, {
        headers: {
          'Content-Type': 'application/json',
          'X-Business-Id': businessId,
        },
      });

      if (response.ok) {
        const data = await response.json();
        setStatus(data);
        onStatusChange?.(data);
      } else {
        throw new Error('Failed to check OAuth status');
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to check OAuth status');
    } finally {
      setLoading(false);
    }
  };

  const handleAuthorize = async () => {
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
        // Redirect to Google OAuth
        window.location.href = data.authorizationUrl;
      } else {
        throw new Error('Failed to generate authorization URL');
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to start OAuth flow');
      setLoading(false);
    }
  };

  const handleRefresh = async () => {
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
        await checkOAuthStatus();
      } else {
        throw new Error('Failed to refresh token');
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to refresh token');
    } finally {
      setLoading(false);
    }
  };

  const handleRevoke = async () => {
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
        await checkOAuthStatus();
      } else {
        throw new Error('Failed to revoke token');
      }
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to revoke token');
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