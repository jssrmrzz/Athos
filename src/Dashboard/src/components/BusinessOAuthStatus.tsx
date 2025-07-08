import React from 'react';
import { Badge } from '@/components/ui/badge';
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card';
import { CheckCircle, XCircle, AlertCircle, RefreshCw } from 'lucide-react';
import { useGoogleOAuth } from '@/hooks/useGoogleOAuth';

interface BusinessOAuthStatusProps {
  businessId: string;
  className?: string;
}

export const BusinessOAuthStatus: React.FC<BusinessOAuthStatusProps> = ({ businessId, className = '' }) => {
  const { status, loading, error } = useGoogleOAuth({ businessId });

  const getStatusBadge = () => {
    if (loading) {
      return (
        <Badge variant="outline" className="flex items-center gap-1">
          <RefreshCw className="h-3 w-3 animate-spin" />
          Checking...
        </Badge>
      );
    }

    if (error) {
      return (
        <Badge variant="destructive" className="flex items-center gap-1">
          <XCircle className="h-3 w-3" />
          Error
        </Badge>
      );
    }

    if (status.isConnected) {
      return (
        <Badge variant="default" className="flex items-center gap-1 bg-green-100 text-green-800">
          <CheckCircle className="h-3 w-3" />
          Connected
        </Badge>
      );
    }

    if (status.hasToken && status.isExpired) {
      return (
        <Badge variant="secondary" className="flex items-center gap-1 bg-yellow-100 text-yellow-800">
          <AlertCircle className="h-3 w-3" />
          Expired
        </Badge>
      );
    }

    if (status.hasToken && status.isRevoked) {
      return (
        <Badge variant="destructive" className="flex items-center gap-1">
          <XCircle className="h-3 w-3" />
          Revoked
        </Badge>
      );
    }

    return (
      <Badge variant="outline" className="flex items-center gap-1">
        <XCircle className="h-3 w-3" />
        Not Connected
      </Badge>
    );
  };

  const getStatusDescription = () => {
    if (loading) return 'Checking Google My Business connection...';
    if (error) return `Error: ${error}`;
    if (status.isConnected) return 'Google My Business is connected and active';
    if (status.hasToken && status.isExpired) return 'Google My Business token has expired';
    if (status.hasToken && status.isRevoked) return 'Google My Business connection was revoked';
    return 'Google My Business is not connected';
  };

  return (
    <Card className={className}>
      <CardHeader className="pb-3">
        <div className="flex items-center justify-between">
          <CardTitle className="text-sm font-medium">Google My Business</CardTitle>
          {getStatusBadge()}
        </div>
        <CardDescription className="text-xs">
          {getStatusDescription()}
        </CardDescription>
      </CardHeader>
      {status.isConnected && (
        <CardContent className="pt-0">
          <div className="text-xs text-gray-500 space-y-1">
            {status.expiresAt && (
              <div>Expires: {new Date(status.expiresAt).toLocaleDateString()}</div>
            )}
            {status.scope && (
              <div>Scopes: {status.scope.split(' ').length} permissions</div>
            )}
          </div>
        </CardContent>
      )}
    </Card>
  );
};