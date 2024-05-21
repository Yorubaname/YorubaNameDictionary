using Core.Entities;
using Core.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Data;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;

namespace Application.Services
{
    public class BasicAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        private readonly IUserRepository _userRepository;

        public BasicAuthenticationHandler(
            IUserRepository userRepository,
            IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
            _userRepository = userRepository;
        }

        protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (!Request.Headers.ContainsKey("Authorization"))
                return await Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));

            try
            {
                (string username, string password) = DecodeBasicAuthToken(Request.Headers["Authorization"]);

                var matchingUser = await AuthenticateUser(username, password);
                if (matchingUser == null)
                {
                    return await Task.FromResult(AuthenticateResult.Fail("Invalid Username or Password"));
                }

                return await Task.FromResult(AuthenticateResult.Success(GenerateAuthTicket(matchingUser)));
            }
            catch
            {
                return await Task.FromResult(AuthenticateResult.Fail("Invalid Authorization Header"));
            }
        }

        private AuthenticationTicket GenerateAuthTicket(User theUser)
        {
            var claims = new List<Claim> {
                    new(ClaimTypes.NameIdentifier, theUser.Email!),
                    new(ClaimTypes.Name, theUser.Email!),
                };

            claims.AddRange(theUser.Roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var identity = new ClaimsIdentity(claims, Scheme.Name);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, Scheme.Name);
            return ticket;
        }

        private static (string username, string password) DecodeBasicAuthToken(string theToken)
        {
            var authHeader = AuthenticationHeaderValue.Parse(theToken);
            var credentialBytes = Convert.FromBase64String(authHeader.Parameter!);
            var credentials = Encoding.UTF8.GetString(credentialBytes).Split(':');
            return (credentials[0], credentials[1]);
        }

        private async Task<User?> AuthenticateUser(string username, string password)
        {
            var user = await _userRepository.GetUserByEmail(username);

            if (user == null)
            {
                return null;
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(password, user.Password);

            return isPasswordValid ? user : null;
        }
    }
}
