using Athos.ReviewAutomation.Core.Entities;
using Athos.ReviewAutomation.Core.Interfaces;
using Athos.ReviewAutomation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Athos.ReviewAutomation.Infrastructure.Repositories
{
    public class OAuthTokenRepository : IOAuthTokenRepository
    {
        private readonly ReviewDbContext _context;

        public OAuthTokenRepository(ReviewDbContext context)
        {
            _context = context;
        }

        public async Task<BusinessOAuthToken?> GetByBusinessIdAsync(int businessId, string provider = "Google")
        {
            return await _context.BusinessOAuthTokens
                .Include(t => t.Business)
                .FirstOrDefaultAsync(t => t.BusinessId == businessId && t.Provider == provider);
        }

        public async Task<BusinessOAuthToken> SaveAsync(BusinessOAuthToken token)
        {
            var existingToken = await GetByBusinessIdAsync(token.BusinessId, token.Provider);
            
            if (existingToken != null)
            {
                existingToken.AccessToken = token.AccessToken;
                existingToken.RefreshToken = token.RefreshToken;
                existingToken.ExpiresAt = token.ExpiresAt;
                existingToken.UpdatedAt = DateTime.UtcNow;
                existingToken.IsRevoked = false;
                existingToken.Scope = token.Scope;
                
                _context.BusinessOAuthTokens.Update(existingToken);
                await _context.SaveChangesAsync();
                return existingToken;
            }
            else
            {
                token.CreatedAt = DateTime.UtcNow;
                token.UpdatedAt = DateTime.UtcNow;
                _context.BusinessOAuthTokens.Add(token);
                await _context.SaveChangesAsync();
                return token;
            }
        }

        public async Task<bool> DeleteByBusinessIdAsync(int businessId, string provider = "Google")
        {
            var token = await GetByBusinessIdAsync(businessId, provider);
            if (token != null)
            {
                _context.BusinessOAuthTokens.Remove(token);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<bool> RevokeByBusinessIdAsync(int businessId, string provider = "Google")
        {
            var token = await GetByBusinessIdAsync(businessId, provider);
            if (token != null)
            {
                token.IsRevoked = true;
                token.UpdatedAt = DateTime.UtcNow;
                _context.BusinessOAuthTokens.Update(token);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<List<BusinessOAuthToken>> GetExpiredTokensAsync()
        {
            return await _context.BusinessOAuthTokens
                .Include(t => t.Business)
                .Where(t => !t.IsRevoked && t.ExpiresAt <= DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<List<BusinessOAuthToken>> GetActiveTokensByProviderAsync(string provider = "Google")
        {
            return await _context.BusinessOAuthTokens
                .Include(t => t.Business)
                .Where(t => t.Provider == provider && !t.IsRevoked && t.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();
        }
    }
}