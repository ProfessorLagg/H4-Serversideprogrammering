using API.Data;
using API.Data.Model;

using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;

namespace API.Services {
    public class H4AuthService {
        private static readonly TimeSpan SessionLifeTime = TimeSpan.FromHours(1.0);
        private readonly H4serversideTodoContext _dbContext;
        public H4AuthService(H4serversideTodoContext databaseContext) {
            this._dbContext = databaseContext;
        }



        public sealed record class ValidateSessionResult(bool Valid = false, bool Authenticated = false, AccountSession? Session = null);
        public async Task<ValidateSessionResult> ValidateSession(string authHeader) {
            if (string.IsNullOrWhiteSpace(authHeader)) { return new ValidateSessionResult(); }

            int spaceIdx = authHeader.IndexOf(' ');
            if (spaceIdx <= 0) { return new ValidateSessionResult(); }

            string protocolString = authHeader.Substring(0, spaceIdx);
            if (string.IsNullOrWhiteSpace(protocolString)) { return new ValidateSessionResult(); }
            string tokenString = authHeader.Substring(spaceIdx + 1);
            if (string.IsNullOrWhiteSpace(tokenString)) { return new ValidateSessionResult(); }

            if (!Guid.TryParse(tokenString, out Guid token)) { return new ValidateSessionResult(); }
            return await ValidateSession(token);
        }
        public async Task<ValidateSessionResult> ValidateSession(Guid token) {
            //AccountSession? session = await _dbContext.AccountSessions.FirstOrDefaultAsync(x => x.Token == token && x.Active);
            AccountSession? session = await _dbContext.AccountSessions.FirstOrDefaultAsync(x => x.Token == token);
            if (session is null) { return new ValidateSessionResult(); }
            if (!session.Active) { return new ValidateSessionResult(true, false, session); }
            session.LastAuthenticated = DateTime.UtcNow;
            var updateResult = _dbContext.Update(session);
            await _dbContext.SaveChangesAsync();
            return new ValidateSessionResult(true, true, updateResult.Entity);
        }

        public async Task<AccountSession> CreateSessionAsync(Account account) {
            AccountSession session = new();
            session.Account = account;
            session.Active = true;
            var addResult = await _dbContext.AccountSessions.AddAsync(session);
            _ = await _dbContext.SaveChangesAsync();
            AccountSession result = addResult.Entity;
            return result;
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

        public sealed record class AuthResult(bool Found, bool Authenticated, Account? Account = null, AccountSession? Session = null);
        public async Task<AuthResult> AuthenticateAsync(string login, string passwordHash) {
            Account? account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Login == login);
            if (account is null) {
                return new AuthResult(false, false);
            }

            bool passwordHashMatches = account.PasswordHash.Equals(passwordHash, StringComparison.InvariantCultureIgnoreCase);
            if (!passwordHashMatches) {
                return new AuthResult(true, false, account);
            }

            AccountSession session = await GetOrCreateSessionAsync(account);

            return new AuthResult(true, true, account, session);
        }
    }
}
