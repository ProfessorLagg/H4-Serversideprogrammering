using API.Data;
using API.Data.Model;

using Microsoft.EntityFrameworkCore;

namespace API.Services {
    public class H4AuthService {
        private static readonly TimeSpan SessionLifeTime = TimeSpan.FromHours(1.0);
        private readonly H4serversideTodoContext _dbContext;
        public H4AuthService(H4serversideTodoContext databaseContext) {
            this._dbContext = databaseContext;
        }

        public sealed class AuthResult {
            public bool Found { get; set; } = false;
            public bool Authenticated { get; set; } = false;
            public Account? Account { get; set; } = null!;
            public AccountSession? Session { get; set; } = null!;
        }

        public async Task<AccountSession> CreateSessionAsync(Account account) {
            AccountSession session = new();
            session.Account = account;
            session.LastAuthenticated = DateTime.UtcNow;
            var addResult = await _dbContext.AccountSessions.AddAsync(session);
            _ = await _dbContext.SaveChangesAsync();
            AccountSession result = addResult.Entity;
            return result;
        }
        public async Task<bool> ValidateSessionToken(Guid token) {
            AccountSession? session = await _dbContext.AccountSessions.FirstOrDefaultAsync(x => x.Token == token && x.Active);
            if (session is null) { return false; }

            session.LastAuthenticated = DateTime.UtcNow;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        public async Task<AccountSession> GetOrCreateSessionAsync(Account account) {
            if (account == null) { throw new ArgumentNullException(nameof(account)); }
            AccountSession? session = await _dbContext.AccountSessions.FirstOrDefaultAsync(x => x.AccountId == account.Id && x.Active);
            if (session is null) { return await CreateSessionAsync(account); }
            if (session.LastAuthenticated + SessionLifeTime >= DateTime.UtcNow) {
                session.Active = false;
                _dbContext.AccountSessions.Update(session);
                return await CreateSessionAsync(account);
            }


            return session;
        }
        public async Task<AuthResult> AuthenticateAsync(string login, string passwordHash) {
            AuthResult result = new();
            Account? account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Login == login);
            if (account is null) {
                result.Found = false;
                return result;
            }
            result.Found = true;
            result.Account = account;

            bool passwordHashMatches = account.PasswordHash.Equals(passwordHash, StringComparison.InvariantCultureIgnoreCase);
            if (!passwordHashMatches) {
                result.Authenticated = false;
                return result;
            }
            result.Authenticated = true;
            result.Session = await GetOrCreateSessionAsync(result.Account);

            return result;
        }
    }
}
