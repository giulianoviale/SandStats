using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SandStats.Data; // ApplicationUser

namespace SandStats.Security
{
    public class AppSignInManager : SignInManager<ApplicationUser>
    {
        public AppSignInManager(
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor contextAccessor,
            IUserClaimsPrincipalFactory<ApplicationUser> claimsFactory,
            IOptions<IdentityOptions> optionsAccessor,
            ILogger<SignInManager<ApplicationUser>> logger,
            IAuthenticationSchemeProvider schemes,
            IUserConfirmation<ApplicationUser> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation) { }

        public override async Task<SignInResult> PasswordSignInAsync(
            string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            // Permitir login por usuario o email
            var user = await UserManager.FindByNameAsync(userName)
                       ?? await UserManager.FindByEmailAsync(userName);

            if (user is null) return SignInResult.Failed;

            // ✅ Si querés controlar acceso con un flag:
            // if (!user.IsActive) return SignInResult.NotAllowed;   // requiere campo IsActive en ApplicationUser

            // Respetar RequireConfirmedAccount si lo activaste en Program.cs
            if (UserManager.Options.SignIn.RequireConfirmedAccount &&
                !await UserManager.IsEmailConfirmedAsync(user))
            {
                return SignInResult.NotAllowed;
            }

            var result = await base.PasswordSignInAsync(user, password, isPersistent, lockoutOnFailure);

            // Auditoría opcional (si agregaste LastLoginAt en ApplicationUser)
            // if (result.Succeeded) {
            //     user.LastLoginAt = DateTime.UtcNow;
            //     await UserManager.UpdateAsync(user);
            // }

            return result;
        }
    }
}
