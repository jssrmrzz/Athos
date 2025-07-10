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

### Backend (.NET)
```bash
# Build the entire solution
dotnet build

# Run the API server (from root directory)
dotnet run --project src/ReviewAutomation/Api/Athos.ReviewAutomation.Api.csproj

# Create migrations (from Api directory)
dotnet ef migrations add MigrationName --project ../Infrastructure --context ReviewDbContext

# Run migrations
dotnet ef database update --project ../Infrastructure --context ReviewDbContext

# Run tests (if test project exists)
dotnet test
```

### Frontend (React)
```bash
# Install dependencies
cd src/Dashboard && npm install

# Start development server
cd src/Dashboard && npm run dev

# Build for production
cd src/Dashboard && npm run build

# Run linting
cd src/Dashboard && npm run lint
```

## Google OAuth Configuration

The application now supports Google My Business OAuth integration for automated review sync:

### Configuration Setup
1. **Google Cloud Console**:
   - Create or use existing Google Cloud project
   - Enable Google My Business API
   - Create OAuth 2.0 credentials (Web Application)
   - Set authorized redirect URI: `http://localhost:7157/auth/google/callback`

2. **Update appsettings.json**:
   ```json
   "GoogleOAuth": {
     "ClientId": "your-google-client-id",
     "ClientSecret": "your-google-client-secret",
     "RedirectUri": "http://localhost:7157/auth/google/callback",
     "Scopes": [
       "https://www.googleapis.com/auth/business.manage",
       "https://www.googleapis.com/auth/business.reviews",
       "https://www.googleapis.com/auth/business.profile"
     ]
   }
   ```

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
- `GET /api/oauth/google/authorize` - Generate OAuth authorization URL
- `GET /api/oauth/google/callback` - Handle OAuth callback with code
- `POST /api/oauth/google/refresh` - Refresh expired OAuth tokens
- `POST /api/oauth/google/revoke` - Revoke OAuth tokens
- `GET /api/oauth/google/status` - Get OAuth connection status

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
- **Google OAuth**: ✅ COMPLETED - Full multi-tenant OAuth integration implemented
- **Frontend Updates**: ✅ COMPLETED - Business settings UI and navigation implemented
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

