# Google OAuth Setup Guide

## Current Status: Basic OAuth Testing ✅

The OAuth system is configured with **basic Google scopes** for immediate testing:
- `https://www.googleapis.com/auth/userinfo.profile`
- `https://www.googleapis.com/auth/userinfo.email`

These scopes work immediately without special verification.

## Testing Your OAuth Flow

1. **Start the backend server**: `dotnet run` from `src/ReviewAutomation/Api/`
2. **Use Business ID 1** in your frontend (X-Business-Id header)
3. **OAuth should work** without "invalid_scope" errors

## Upgrading to Google My Business API Scopes

When you're ready to access Google My Business data, you'll need to:

### 1. Enable Google My Business API
In your Google Cloud Console:
1. Go to [Google Cloud Console APIs](https://console.cloud.google.com/apis/library)
2. Search for "Google My Business API"
3. Click "Enable" for your project

### 2. Configure OAuth Consent Screen
1. Go to [OAuth Consent Screen](https://console.cloud.google.com/apis/credentials/consent)
2. Add these scopes:
   - `https://www.googleapis.com/auth/business.manage`
   - `https://www.googleapis.com/auth/business.reviews`
   - `https://www.googleapis.com/auth/business.profile`
3. **Important**: You may need to submit for Google verification for sensitive scopes

### 3. Update appsettings.json
Replace the current scopes in `src/ReviewAutomation/Api/appsettings.json`:

```json
{
  "GoogleOAuth": {
    "Scopes": [
      "https://www.googleapis.com/auth/business.manage",
      "https://www.googleapis.com/auth/business.reviews", 
      "https://www.googleapis.com/auth/business.profile"
    ]
  }
}
```

### 4. Business Verification Requirements
- **Google My Business listing** must be verified
- **Business ownership** must be proven to Google
- **API access** may require additional verification for production use

## Development vs Production

### Current Setup (Development)
- ✅ Basic OAuth scopes (profile, email)
- ✅ Works immediately for testing
- ✅ No special verification needed

### Production Ready (Future)
- 🔄 Google My Business API scopes
- 🔄 API verification process
- 🔄 Business listing verification
- 🔄 Production OAuth consent screen approval

## Troubleshooting

### "invalid_scope" Error
- **Cause**: Requesting scopes not enabled in Google Cloud Console
- **Solution**: Use basic scopes first, then upgrade step by step

### "access_denied" Error  
- **Cause**: OAuth consent screen not properly configured
- **Solution**: Check OAuth consent screen settings in Google Cloud Console

### "redirect_uri_mismatch" Error
- **Cause**: Redirect URI doesn't match Google Cloud Console configuration
- **Current URI**: `https://localhost:7157/api/oauth/google/callback`
- **Solution**: Ensure this exact URI is in your Google Cloud Console OAuth settings

## Next Steps

1. **Test basic OAuth flow** with current setup
2. **Verify Google Cloud Console** configuration
3. **Enable Google My Business API** when ready for business data
4. **Submit for verification** if required for production use

The system is ready for immediate OAuth testing with basic Google scopes!