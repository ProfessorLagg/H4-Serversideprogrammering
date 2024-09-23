using API.Data;
using API.Data.Model;

using Microsoft.EntityFrameworkCore;

namespace API.Services {
    public class H4AuthService {
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

        public async Task<AccountSession> GetOrCreateSession(Account account) {
            if (account == null) { throw new ArgumentNullException(nameof(account)); }
            AccountSession? session = await _dbContext.AccountSessions.FirstOrDefault(s => s.AccountId == account)

        }
        public async Task<AuthResult> AuthenticateAsync(string login, string passwordHash) {
            AuthResult result = new();
            Account? account = await _dbContext.Accounts.FirstOrDefaultAsync(x => x.Login == login);
            if (account == null) {
                result.Found = false;
                return result;
            }
            result.Found = true;

            bool passwordHashMatches = account.PasswordHash.Equals(passwordHash, StringComparison.InvariantCultureIgnoreCase);
            if (!passwordHashMatches) {
                result.Authenticated = false;
                return result;
            }
            result.Authenticated = true;


            return result;
        }
    }
}
