# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Development Workflow

### Process Steps
1. **Analysis & Planning** - Analyze requirements and create detailed plan in `tasks/todo.md`
2. **Plan Verification** - Get approval before starting implementation
3. **Incremental Execution** - Work through todo items one by one
4. **Activity Logging** - Document all actions in `docs/activity.md`
5. **Progress Communication** - Provide high-level summaries of changes
6. **Final Review** - Add summary section to `tasks/todo.md`

### Core Principles
- **Simplicity First** - Minimize code impact and complexity
- **Incremental Progress** - Small, focused changes
- **Clear Communication** - Regular updates and explanations
- **Comprehensive Documentation** - Track all activities and decisions

### File Structure
- `tasks/todo.md` - Current project tasks and completion status
- `docs/activity.md` - Development activity log
- `CLAUDE.md` - Project guidance and workflow (this file)

## Architecture Overview

Athos is a **multi-tenant SaaS platform** for automated review management with two main components:

1. **Backend (.NET 6 Web API)** - Located in `src/ReviewAutomation/`
   - **Multi-Tenant Clean Architecture** with layered separation:
     - **Api**: Controllers, middleware, and HTTP endpoints
     - **Application**: Use cases and business logic
     - **Core**: Domain entities, interfaces, and business rules
     - **Infrastructure**: Data access, LLM clients, external services
     - **Models**: DTOs and data transfer objects

2. **Frontend (React + TypeScript + Vite)** - Located in `src/Dashboard/`
   - Modern React application with Tailwind CSS
   - Radix UI components for consistent design
   - Mock API mode for development/testing

## Key Features

### Multi-Tenant SaaS Capabilities
- **Business Management**: Complete business onboarding and configuration
- **User Management**: Role-based access control (Owner, Admin, Manager, Viewer)
- **Data Isolation**: Automatic tenant scoping for complete security
- **Subscription Tiers**: Support for different business plans
- **Business Settings**: Per-tenant configuration and preferences

### Review Management
- **Review Processing**: Fetch, filter, sort, and paginate Google reviews
- **LLM Integration**: Generate AI-powered response suggestions with fallback handling
- **Sentiment Analysis**: Categorize reviews as positive, neutral, or negative
- **SMS Notifications**: Business-configurable text notifications
- **Dual API Mode**: Switch between real API and mock data for development

### Enterprise Features
- **Mobile-Friendly**: Responsive design with mobile device support
- **OAuth Ready**: Infrastructure prepared for Google Business Profile integration
- **Unified Authentication**: Consistent sign out/disconnect experience across the application
- **Audit Trail**: Comprehensive activity logging and user tracking
- **API Security**: Business context validation and authorization

## Common Development Commands

### ⚠️ Development Server (from Project Root)
```bash
# Start both frontend and backend simultaneously (RECOMMENDED)
npm run dev

# Alternative: Start individual services
npm run backend  # .NET API only
npm run frontend # React UI only

# Debug mode with verbose logging
npm run dev:debug

# Install all dependencies
npm run install:all
```

### Backend (.NET)
```bash
# Build the entire solution (from root directory)
dotnet build

# Run the API server (from root directory)
dotnet run --project src/ReviewAutomation/Api/Athos.ReviewAutomation.Api.csproj

# Create migrations (from Api directory)
cd src/ReviewAutomation/Api
dotnet ef migrations add MigrationName --project ../Infrastructure --context ReviewDbContext

# Run migrations (from Api directory)
dotnet ef database update --project ../Infrastructure --context ReviewDbContext

# Run tests (if test project exists, from root)
dotnet test
```

### Frontend (React)
```bash
# Install dependencies (from project root)
npm run install:frontend
# Or manually: cd src/Dashboard && npm install

# Start development server (from project root)
npm run frontend
# Or manually: cd src/Dashboard && npm run dev

# Build for production (from project root)
cd src/Dashboard && npm run build

# Run linting (from Dashboard directory)
cd src/Dashboard && npm run lint
```

## Google OAuth Configuration

The application supports Google My Business OAuth integration for automated review sync with conditional registration for development flexibility:

### Configuration Setup
1. **Google Cloud Console**:
   - Create or use existing Google Cloud project
   - Enable Google My Business API
   - Create OAuth 2.0 credentials (Web Application)
   - Set authorized redirect URI: `https://localhost:7157/api/oauth/google/callback`

2. **Update appsettings.json** (Optional - for OAuth functionality):
   ```json
   "GoogleOAuth": {
     "ClientId": "your-google-client-id",
     "ClientSecret": "your-google-client-secret",
     "RedirectUri": "https://localhost:7157/api/oauth/google/callback",
     "Scopes": [
       "https://www.googleapis.com/auth/business.manage",
       "https://www.googleapis.com/auth/business.reviews",
       "https://www.googleapis.com/auth/business.profile"
     ]
   }
   ```

**Note**: OAuth configuration is optional for development. Leave `ClientId` and `ClientSecret` empty to use mock mode without OAuth dependencies.

### OAuth Flow
1. **Authorization**: Business owner clicks "Connect Google My Business"
2. **Redirect**: User redirected to Google OAuth consent screen
3. **Callback**: Google redirects back with authorization code
4. **Token Exchange**: Backend exchanges code for access/refresh tokens
5. **Token Storage**: Tokens stored with business-level scoping
6. **API Access**: Authenticated calls to Google My Business API

### Features
- **Multi-Tenant**: Each business has separate OAuth tokens
- **Auto-Refresh**: Expired tokens automatically refreshed
- **Fallback**: Graceful degradation to mock API if OAuth fails
- **Status Monitoring**: Real-time connection status in settings UI
- **Server-Side Flow**: Prevents CSP errors and enhances security
- **Profile Integration**: User profile display with fallback handling

## OAuth Implementation Details

### Architecture Overview

Athos implements a **server-side OAuth 2.0 flow** for Google Business Profile integration, designed specifically for multi-tenant SaaS requirements.

#### Key Design Decisions

**Server-Side Redirect Pattern**:
- Frontend navigates directly to `/api/oauth/google/authorize?businessId=1`
- Backend performs HTTP 302 redirect to Google OAuth
- Eliminates Content Security Policy (CSP) errors
- Prevents client-side JavaScript `eval()` issues
- Enhances security by keeping OAuth URLs server-side

**Multi-Tenant Business Context**:
- Business ID passed via query parameter
- BusinessContextMiddleware handles business scoping
- State parameter preserves business context through OAuth flow
- Automatic business validation and token isolation

### OAuth Flow Implementation

#### 1. Authorization Request
```
GET /api/oauth/google/authorize?businessId=1
```
- BusinessContextMiddleware extracts businessId from query parameter
- Sets business context claims (BusinessId, UserId, Role)
- GoogleOAuthService generates authorization URL with business state
- Server returns HTTP 302 redirect to Google OAuth consent

#### 2. Google OAuth Consent
- User sees consent screen with scopes:
  - `https://www.googleapis.com/auth/business.manage` - Google Business Profile access
  - `https://www.googleapis.com/auth/userinfo.profile` - User profile information
  - `https://www.googleapis.com/auth/userinfo.email` - User email access

#### 3. OAuth Callback
```
GET /api/oauth/google/callback?code=...&state=1
```
- Extract business ID from state parameter
- Exchange authorization code for access/refresh tokens
- Save tokens to database with business scoping
- Redirect to frontend: `http://localhost:5173/business/settings?oauth=success&businessId=1`

#### 4. Frontend Integration
- GoogleOAuthButton detects oauth=success parameter
- Displays success toast notification
- Updates connection status to "Connected"
- Fetches user profile for display

### Technical Components

#### Backend Services
- **OAuthController**: Handles authorization and callback endpoints
- **GoogleOAuthService**: Token lifecycle management (exchange, refresh, revoke)
- **OAuthTokenRepository**: Database persistence with business scoping
- **BusinessContextMiddleware**: Multi-tenant request context handling
- **AuthenticatedGoogleApiClient**: OAuth-enabled Google API client

#### Frontend Components  
- **GoogleOAuthButton**: Connection UI with real-time status
- **BusinessDropdown**: User profile display with image fallbacks
- **useGoogleOAuth**: OAuth state management hook
- **useOAuthUser**: User profile and authentication context

#### Database Schema
- **BusinessOAuthTokens**: Business-scoped OAuth token storage
  - BusinessId (foreign key)
  - Provider ("Google")
  - AccessToken, RefreshToken
  - ExpiresAt, Scope, IsRevoked
  - Automatic business-level isolation

### Security Features

#### Multi-Tenant Isolation
- All OAuth tokens scoped by BusinessId
- Cross-tenant access prevention via middleware
- Business context validation on all endpoints
- Row-level security for database queries

#### OAuth Security
- Server-side token handling (no client exposure)
- Automatic token refresh for expired tokens
- Secure token revocation and cleanup
- State parameter validation for CSRF protection

### Error Handling & Troubleshooting

#### Common Issues and Solutions

**CSP `eval()` Errors**: 
- ✅ Solved: Server-side redirect eliminates client-side JavaScript
- Legacy client-side approaches would fail with CSP restrictions

**Invalid OAuth Scopes**:
- ✅ Solved: Using only valid Google Business Profile scopes
- Removed deprecated `business.profile` and `business.reviews` scopes

**Business Context Not Found**:
- ✅ Solved: Middleware reads businessId from query parameters
- Supports both header and query parameter business context

**SSL Protocol Errors**:
- ✅ Solved: Callback redirects match frontend protocol (HTTP vs HTTPS)
- Dynamic protocol detection for development/production

**Profile Image Fallbacks**:
- ✅ Implemented: Graceful fallback to User icon when Google images fail
- Error handling prevents broken image display

### Production Deployment

#### Google Cloud Console Setup
1. Create Google Cloud project
2. Enable Google My Business API
3. Create OAuth 2.0 credentials (Web Application)
4. Set authorized redirect URI: `https://yourdomain.com/api/oauth/google/callback`

#### Configuration
Update `appsettings.json`:
```json
{
  "GoogleOAuth": {
    "ClientId": "your-google-client-id",
    "ClientSecret": "your-google-client-secret", 
    "RedirectUri": "https://yourdomain.com/api/oauth/google/callback",
    "Scopes": [
      "https://www.googleapis.com/auth/business.manage",
      "https://www.googleapis.com/auth/userinfo.profile",
      "https://www.googleapis.com/auth/userinfo.email"
    ]
  }
}
```

#### Environment Considerations
- **Development**: Uses `http://localhost:5173` for frontend redirects
- **Production**: Update callback URLs to use HTTPS and production domains
- **Mobile**: CORS policy includes mobile IP addresses for development
- **Scaling**: OAuth tokens are business-scoped for multi-tenant scaling

## API Endpoints

The backend runs on `http://localhost:7157` (or `http://0.0.0.0:7157` for mobile access):

### Business Management
- `GET /api/business` - Get user's businesses
- `POST /api/business` - Create new business
- `GET /api/business/{id}` - Get specific business details
- `PUT /api/business/{id}` - Update business settings
- `GET /api/business/{id}/members` - Get business members
- `POST /api/business/{id}/members` - Invite user to business
- `PUT /api/business/{id}/members/{userId}/role` - Update member role
- `DELETE /api/business/{id}/members/{userId}` - Remove member

### Review Management (Business-Scoped)
- `GET /api/reviews` - Fetch paginated reviews for current business
- `POST /api/reviews/respond` - Submit final response for a review
- `POST /api/reviews/import-google` - Import reviews from Google API
- `POST /api/llm/suggest` - Generate AI response suggestions

### Google OAuth Integration
- `GET /api/oauth/google/authorize?businessId={id}` - Server-side OAuth redirect (supports multi-tenant)
- `GET /api/oauth/google/callback` - Handle OAuth callback and token exchange
- `POST /api/oauth/google/refresh` - Refresh expired OAuth tokens
- `POST /api/oauth/google/revoke` - Revoke OAuth tokens and disconnect
- `GET /api/oauth/google/status` - Get OAuth connection status with user profile

### System
- `GET /api/health` - Health check endpoint

## Frontend Configuration

The React app includes:
- **Mock API Toggle**: Switch between real API (`/api/*`) and mock API (`/api/mock/*`)
- **Mobile Support**: Automatic IP detection for mobile device access (fallback to `10.0.0.22`)
- **Dark Mode**: Theme switching capability
- **Toast Notifications**: User feedback system

- **Business Navigation**: Interactive business management dropdown with routing
- **React Router**: Complete routing structure for business management pages

- **Navigation**: React Router with business owner dropdown menu
- **Business Settings**: Complete OAuth management interface
- **Real-time Status**: Live OAuth connection monitoring


## Database

- **SQLite Database**: `reviews.db` stores multi-tenant data
- **Entity Framework**: Code-first migrations in `src/ReviewAutomation/Infrastructure/Migrations/`
- **Multi-Tenant Schema**: All data automatically scoped by BusinessId
- **Key Tables**:
  - `Businesses` - Tenant configurations and settings
  - `Users` - Global user accounts
  - `BusinessUsers` - User-business relationships with roles
  - `Reviews` - Business-scoped review data
  - `BusinessSettings` - Per-tenant preferences
  - `BusinessOAuthTokens` - Business-specific OAuth credentials
- **Seeding**: Mock data loaded from `src/ReviewAutomation/Infrastructure/Data/mockGoogleReviews.json`

## LLM Integration

- **Multiple Providers**: OpenAI and Local LLM support
- **Resilient Client**: Automatic retry logic with exponential backoff
- **Fallback Handling**: Graceful degradation when LLM services fail

## Development Notes

### Multi-Tenant Development
- **Business Context**: All requests automatically include business context via middleware
- **Data Isolation**: All repositories automatically filter by BusinessId
- **Role Validation**: Controllers use business context service for permission checks
- **Business Switching**: Support for users belonging to multiple businesses

### General Development
- **CORS Configuration**: Frontend allowed origins include localhost and mobile IP addresses
- **Mock Mode**: Default development mode uses mock data for consistent testing
- **Clean Architecture**: Follow existing patterns when adding new features
- **Error Handling**: Comprehensive error responses with user-friendly messages
- **Authentication**: Infrastructure ready for Google OAuth integration

### Security Considerations
- **Row-Level Security**: All data queries automatically scoped by BusinessId
- **Business Validation**: Middleware prevents cross-tenant data access
- **Role Hierarchy**: Permission system with Owner > Admin > Manager > Viewer
- **API Authorization**: All business endpoints require proper business context

## Project Structure

```
src/
├── ReviewAutomation/           # Multi-Tenant Backend API
│   ├── Api/                    # Controllers, middleware, Program.cs
│   │   ├── Controllers/        # Business and Review controllers
│   │   └── Middleware/         # BusinessContextMiddleware
│   ├── Application/            # Business-scoped use cases, DTOs
│   ├── Core/                   # Multi-tenant domain entities, interfaces
│   │   ├── Entities/           # Business, User, BusinessUser, Reviews
│   │   └── Interfaces/         # Business and review repositories
│   ├── Infrastructure/         # Data access, services, LLM clients
│   │   ├── Data/               # ReviewDbContext with multi-tenant entities
│   │   ├── Repositories/       # Business-scoped repositories
│   │   ├── Services/           # BusinessContextService
│   │   └── Migrations/         # EF Core migrations
│   └── Models/                 # Shared DTOs
└── Dashboard/                  # Frontend React app (to be updated for multi-tenant)
    ├── src/
    │   ├── components/         # UI components + business management (future)
    │   ├── context/            # React contexts + business context (future)
    │   ├── hooks/              # Custom hooks + business hooks (future)
    │   ├── pages/              # Route components
    │   └── lib/                # Utilities
    └── public/                 # Static assets
```

## Multi-Tenant Architecture Details

### Business Context Flow
1. **Request Processing**: BusinessContextMiddleware extracts business context
2. **User Validation**: Validates user has access to requested business
3. **Context Injection**: Sets BusinessId in request context
4. **Repository Filtering**: All data queries automatically filtered by BusinessId
5. **Response**: Business-scoped data returned

### Role-Based Access Control
- **Owner**: Full business control, user management, settings
- **Admin**: User management, business settings, review management
- **Manager**: Review management, response approval
- **Viewer**: Read-only access to reviews and reports

### Data Security
- **Automatic Scoping**: All queries include BusinessId filter
- **Cross-Tenant Prevention**: Middleware blocks unauthorized business access
- **Row-Level Security**: Database-level tenant isolation
- **Audit Trail**: User actions tracked per business


### Future Implementation Notes
- **Google OAuth**: Infrastructure ready for Google Business Profile integration
- **Real Google API**: Switch from mock to actual Google My Business API
- **Analytics**: Per-business usage tracking and insights
- **Billing**: Subscription management and payment processing

### Implementation Status
- **Google OAuth**: ✅ PRODUCTION READY - Complete server-side OAuth 2.0 flow with multi-tenant support
- **Frontend Integration**: ✅ COMPLETED - Business settings UI, navigation, and user profile display
- **Security & Error Handling**: ✅ COMPLETED - CSP compliance, fallback handling, comprehensive logging
- **Real Google API**: ✅ READY - OAuth authentication ready for live Google My Business API
- **Analytics**: Per-business usage tracking and insights (future)
- **Billing**: Subscription management and payment processing (future)


## Mobile Development

The application is configured for mobile testing:
- API server binds to `0.0.0.0:7157` for network access
- Frontend automatically detects mobile environment
- CORS policy includes mobile IP addresses
- Responsive design optimized for mobile viewing

## Recent Major Changes

### Multi-Tenant Transformation (2025-07-07)
- **Architecture**: Converted from single-tenant to multi-tenant SaaS platform
- **Database**: Added Business, User, BusinessUser, BusinessSettings tables
- **Security**: Implemented row-level security with automatic BusinessId scoping
- **APIs**: Added comprehensive business management endpoints
- **Middleware**: Created BusinessContextMiddleware for tenant isolation
- **Migration**: Successfully created AddMultiTenantSupport migration


### Business Navigation Enhancement (2025-07-08)
- **Frontend**: Fixed non-functional "Business Owner" button in Dashboard
- **Components**: Created BusinessDropdown with interactive navigation menu
- **Pages**: Built BusinessSettings, BusinessUsers, and BusinessProfile pages
- **Routing**: Implemented complete React Router structure for business management
- **UX**: Added click-outside-to-close and dark mode support

### Key Files Modified
- **Multi-Tenant Backend**: 20+ files updated across Core, Infrastructure, Application, and API layers
- **Business Navigation**: 7 files created/modified in Dashboard frontend components
- New entities: Business, User, BusinessUser, BusinessSettings, BusinessOAuthToken
- Updated ReviewsController and repositories for business scoping
- Added BusinessController for complete business management
- Enhanced dependency injection with new services

### Google OAuth Integration (2025-07-08)
- **OAuth Infrastructure**: Complete multi-tenant OAuth 2.0 implementation
- **Backend Services**: GoogleOAuthService with full token lifecycle management
- **API Endpoints**: `/api/oauth/google/*` endpoints for auth flow
- **Frontend Integration**: Business settings UI with OAuth management
- **Navigation**: Added React Router with dropdown menu navigation
- **Security**: Business-scoped OAuth tokens with automatic refresh
- **Error Handling**: Comprehensive error handling and user feedback

### Dashboard Navigation Cleanup (2025-07-08)
- **UI Simplification**: Removed duplicate "Business Owner" dropdown from Topbar
- **Sign Out Integration**: Added proper sign out functionality to BusinessDropdown
- **Authentication Hook**: Created useAuth hook for authentication state management
- **User Experience**: Consolidated navigation into single, intuitive dropdown
- **Code Cleanup**: Removed unused imports and redundant components

### Unified Sign Out Implementation (2025-07-10)
- **Problem Resolution**: Fixed non-working Sign Out button that was calling ASP.NET Core logout instead of OAuth revocation
- **Unified Authentication**: Made Sign Out button work the same as Disconnect feature in Business Settings
- **Enhanced UX**: Added loading states, toast notifications, and proper error handling to Sign Out process
- **Clear Authentication Model**: Established single authentication concept where Connected = Authenticated
- **Consistent Behavior**: Sign Out now properly revokes Google OAuth tokens and clears user session

### Mock Mode OAuth Configuration Fix (2025-07-10)
- **Problem Resolution**: Fixed mock mode failing due to OAuth configuration errors during application startup
- **Conditional OAuth Registration**: Updated Program.cs to only register Google OAuth when credentials are configured
- **Mock API Middleware**: Enhanced BusinessContextMiddleware to skip /api/mock endpoints without authentication
- **Development Experience**: Mock mode now works independently without requiring OAuth credentials
- **OAuth Preservation**: Full OAuth functionality preserved when proper credentials are provided

### Key Files Added/Modified
**Backend (.NET 6)**:
- `IOAuthTokenRepository` and `OAuthTokenRepository` - Token management
- `IGoogleOAuthService` and `GoogleOAuthService` - OAuth flow implementation
- `OAuthController` - OAuth API endpoints
- `AuthenticatedGoogleApiClient` - OAuth-enabled Google API client
- `BusinessContextMiddleware` - Enhanced OAuth endpoint handling

**Frontend (React + TypeScript)**:
- `GoogleOAuthButton` - OAuth connection component
- `useGoogleOAuth` - OAuth status management hook
- `BusinessSettingsPage` - Complete settings UI with OAuth integration
- `dropdown-menu.tsx` - Business owner dropdown navigation
- `tabs.tsx` - Settings page tab navigation
- Enhanced `App.tsx` with React Router
- Updated `Topbar.tsx` with navigation dropdown

**Navigation Cleanup (2025-07-08)**:
- `BusinessDropdown.tsx` - Added sign out functionality and LogOut icon
- `Topbar.tsx` - Removed duplicate Business Owner dropdown
- `useAuth.ts` - Created authentication hook for sign out logic

**Unified Sign Out Implementation (2025-07-10)**:
- `useAuth.ts` - Updated to call OAuth revoke endpoint instead of ASP.NET logout
- `BusinessDropdown.tsx` - Enhanced with loading states, toast notifications, and proper UX

**Mock Mode OAuth Configuration Fix (2025-07-10)**:
- `Program.cs` - Added conditional OAuth registration to prevent startup errors
- `BusinessContextMiddleware.cs` - Added /api/mock to skip list for authentication bypass
- `App.tsx` - Cleaned up duplicate routes and unused imports

### OAuth Configuration Fix (2025-07-09)
- **Root Cause**: HTTP/HTTPS protocol mismatch between frontend and backend OAuth endpoints
- **Resolution**: Updated frontend to use HTTPS protocol matching backend server
- **Route Consistency**: Ensured all OAuth components use consistent redirect URIs
- **Enhanced Logging**: Added comprehensive error logging to OAuth endpoints for debugging
- **Configuration Validation**: Verified Google Cloud Console, appsettings.json, and controller routes alignment

### Key Files Modified
**Frontend Protocol Fix**:
- `useApi.ts` - Updated from HTTP to HTTPS protocol for all API calls

**Backend Configuration**:
- `appsettings.json` - Corrected redirect URI consistency
- `OAuthController.cs` - Enhanced error logging and debugging capabilities

**Result**: ERR_EMPTY_RESPONSE OAuth errors resolved, system ready for Google OAuth testing

### Complete OAuth Integration Implementation (2025-08-04)
- **Architecture**: Implemented production-ready server-side OAuth 2.0 flow for Google Business Profile
- **Multi-Tenant**: Full business-scoped OAuth token management with automatic isolation
- **Security**: Resolved CSP eval() errors through server-side redirect pattern
- **Scope Management**: Configured optimal Google OAuth scopes (business.manage, userinfo.profile, userinfo.email)
- **Error Handling**: Comprehensive troubleshooting and fallback mechanisms
- **User Experience**: Seamless authentication with profile display and connection status
- **Production Ready**: Complete deployment configuration and documentation

### OAuth Implementation Challenges Resolved
**CSP Protocol Errors (Content Security Policy)**:
- **Problem**: Client-side OAuth redirects triggered CSP eval() violations
- **Solution**: Implemented server-side HTTP 302 redirect pattern
- **Impact**: Eliminated browser security restrictions and improved user experience

**Invalid OAuth Scopes**:
- **Problem**: Google rejected deprecated business.profile and business.reviews scopes
- **Solution**: Updated to valid scopes (business.manage, userinfo.profile, userinfo.email)
- **Impact**: Successful OAuth consent and token acquisition

**Business Context Issues**:
- **Problem**: Multi-tenant business context not preserved through OAuth flow
- **Solution**: Enhanced BusinessContextMiddleware to support query parameters
- **Impact**: Proper business scoping and token isolation

**SSL Protocol Mismatch**:
- **Problem**: HTTPS/HTTP protocol mismatch causing ERR_SSL_PROTOCOL_ERROR
- **Solution**: Dynamic protocol detection and environment-specific redirects
- **Impact**: Seamless redirect handling across development and production

**Profile Image Handling**:
- **Problem**: Broken profile image display when Google image URLs fail
- **Solution**: Implemented graceful fallback to User icon with proper styling
- **Impact**: Professional appearance regardless of image load success

**Merge Regression Recovery**:
- **Problem**: Code merge reverted critical OAuth implementations
- **Solution**: Systematic restoration of server-side redirect, scopes, and middleware
- **Impact**: Maintained production-ready OAuth integration

### Key Files Modified
**Backend OAuth Implementation**:
- `OAuthController.cs` - Server-side redirect with comprehensive logging
- `GoogleOAuthService.cs` - Token lifecycle management and user profile handling
- `BusinessContextMiddleware.cs` - Multi-tenant query parameter support
- `appsettings.json` - Optimized OAuth scope configuration

**Frontend Integration**:
- `GoogleOAuthButton.tsx` - Direct navigation with fallback image handling
- `BusinessDropdown.tsx` - User profile display with error handling
- `useGoogleOAuth.ts` - OAuth state management hook
- `useOAuthUser.ts` - Authentication context management

**Documentation**:
- `CLAUDE.md` - Comprehensive OAuth implementation documentation
- `docs/activity.md` - Detailed development activity log

