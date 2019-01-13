using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using HBDotCom.Areas.Identity.Models;

namespace HBDotCom.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ExternalLoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ExternalLoginModel> _logger;

        public ExternalLoginModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<ExternalLoginModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string LoginProvider { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [EmailAddress]
            public string Email { get; set; }

            public string UserName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public IActionResult OnGetAsync()
        {
            return RedirectToPage("./Login");
        }

        public IActionResult OnPost(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Page("./ExternalLogin", pageHandler: "Callback", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> OnGetCallbackAsync(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information for user.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider
            // Only if the user already has a login linked with this provider
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                // User has a previous login linked with this provider
                // They are successfully logged in.
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                
                // Here, if we already have a user with the same email address, we'll log them into their account.
                // If they don't have a site account yet I'll ask the user for a password and create the account from that
                // This will utilise the existing scaffolded structure much better and ensure a user has a password.

                ReturnUrl = returnUrl;
                LoginProvider = info.LoginProvider;

                // Email address is the primary key linking the same user across providers.
                // Have we got an email?
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    string email = info.Principal.FindFirstValue(ClaimTypes.Email);

                    // We do have an email - let's check if we have a user in the DB with a matching email
                    var alreadyRegisteredUser = await _userManager.FindByEmailAsync(email);
                    if (alreadyRegisteredUser != null)
                    {
                        // We do have this user in our DB by email
                        // Let's log them in
                        await _signInManager.SignInAsync(alreadyRegisteredUser, isPersistent: false);
                        await _userManager.AddLoginAsync(alreadyRegisteredUser, info);
                        return LocalRedirect(returnUrl);
                    }

                    // We dont have a user matching this email
                    // Let's ask them to set a password in order to create an account
                    Input = new InputModel
                    {
                        Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                    };
                    if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Name))
                    {
                        // We do have a username returned to us by the login provider
                        // We'll also put that username into a variable for easy use below
                        Input.UserName = info.Principal.FindFirstValue(ClaimTypes.Name).Replace(" ", "_");
                    }

                }
                return Page();
            }
        }

        public async Task<IActionResult> OnPostConfirmationAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            // Get the information about the user from the external login provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            if (ModelState.IsValid)
            {
                var email = info.Principal.HasClaim(c => c.Type == ClaimTypes.Name) ? info.Principal.FindFirstValue(ClaimTypes.Email) : Input.Email;
                var userName = info.Principal.HasClaim(c => c.Type == ClaimTypes.Name) ? info.Principal.FindFirstValue(ClaimTypes.Name).Replace(" ", "_") : email;

                var user = new ApplicationUser {
                    UserName = userName,
                    Email = email };
                var result = await _userManager.CreateAsync(user, Input.Password);
                if (result.Succeeded)
                {
                    var identityResult = await _userManager.AddLoginAsync(user, info);
                    if (identityResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            LoginProvider = info.LoginProvider;
            ReturnUrl = returnUrl;
            return Page();
        }
    }
}