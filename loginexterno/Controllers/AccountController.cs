using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace loginexterno.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {

        [HttpGet]
        public IActionResult ExternalLogin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string returnUrl)
        {
            return new ChallengeResult(
                GitHubAuthenticationDefaults.AuthenticationScheme,
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action(nameof(ExternalLoginCallback), new { returnUrl })
                });
        }

        public async Task<IActionResult> ExternalLoginCallback(string returnUrl)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GitHubAuthenticationDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return BadRequest(); // TODO: Handle this better.

            var claimsIdentity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

            foreach (var claim in authenticateResult.Principal.Claims)
            {
                claimsIdentity.AddClaim(claim);
            }

            await HttpContext.SignInAsync(new ClaimsPrincipal(claimsIdentity));

            returnUrl = returnUrl ?? "/";

            return LocalRedirect(returnUrl);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
