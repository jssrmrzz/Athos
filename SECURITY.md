# Security Configuration

## API Keys and Secrets

This project uses several external APIs that require API keys and secrets:

### Google OAuth Configuration
- **Required for**: Google Business Profile integration
- **Get credentials**: [Google Cloud Console](https://console.cloud.google.com)
- **Setup guide**: See `OAUTH_SETUP_GUIDE.md` for detailed instructions

### OpenAI API (Optional)
- **Required for**: OpenAI LLM integration (alternative to local LLM)
- **Get API key**: [OpenAI Platform](https://platform.openai.com/api-keys)

## Local Development Setup

1. **Copy the example configuration**:
   ```bash
   cp src/ReviewAutomation/Api/appsettings.example.json src/ReviewAutomation/Api/appsettings.json
   ```

2. **Edit `appsettings.json` with your credentials**:
   ```json
   {
     "GoogleOAuth": {
       "ClientId": "your-google-client-id-here",
       "ClientSecret": "your-google-client-secret-here"
     },
     "LLM": {
       "OpenAI": {
         "ApiKey": "your-openai-api-key-here"
       }
     }
   }
   ```

3. **For mock mode development** (no APIs needed):
   - Leave OAuth fields empty or as placeholder values
   - Set `"LLMProvider": "Local"` to use local LLM

## What's Protected

✅ **Git ignored** (safe):
- `**/appsettings.json` - Your local configuration with real secrets
- `**/appsettings.*.json` - Environment-specific configurations
- `.env` files with environment variables

✅ **Safe to commit**:
- `appsettings.example.json` - Template with placeholder values
- All other configuration files without secrets

## Production Deployment

For production deployments, consider using:
- **Azure Key Vault** for ASP.NET Core applications
- **AWS Secrets Manager** for AWS deployments  
- **Environment variables** for containerized deployments
- **HashiCorp Vault** for enterprise secret management

Never put production secrets directly in configuration files.