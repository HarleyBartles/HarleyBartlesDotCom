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
            [Required]
            [EmailAddress]
            public string Email { get; set; }
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
                ErrorMessage = "Error loading external login information.";
                return RedirectToPage("./Login", new { ReturnUrl = returnUrl });
            }

            // Sign in the user with this external login provider if the user already has a login linked with this provider
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity.Name, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                // Here I'll try to change the behaviour to automagically create an account.

                ReturnUrl = returnUrl;
                LoginProvider = info.LoginProvider;

                // Have we got an email?
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Email))
                {
                    string email = info.Principal.FindFirstValue(ClaimTypes.Email);

                    // We do have an email - let's check if we have a user in the DB with a matching email
                    var alreadyRegisteredUser = await _userManager.FindByEmailAsync(email);
                    if (alreadyRegisteredUser != null)
                    {
                        // We Do - Login
                        await _signInManager.SignInAsync(alreadyRegisteredUser, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                    // We dont hae a user matching by email, lets create him
                    // Call the private function to create the user
                    bool createUserResult = await PostBackSuccessCreateUserAsync(info, email);

                    // Did that create ok?
                    if (createUserResult)
                    {
                        return LocalRedirect(returnUrl);
                    }
                    else
                    {
                        // Oops. Didn't create. Put the email address into the InputModel.
                        Input = new InputModel
                        {
                            Email = info.Principal.FindFirstValue(ClaimTypes.Email)
                        };
                    }
                }
                // User was not created. Return the confirmation page.
                return Page();
            }
        }

        private async Task<bool> PostBackSuccessCreateUserAsync(ExternalLoginInfo info, string email)
        {
            if (info == null)
            {
                ErrorMessage = "Error loading external login information during confirmation.";
                return false;
            }

            if (email == null)
            {
                ErrorMessage = "Error. No Email Address";
                return false;
            }
            {
                // Some locals
                string userName = "";
                bool hasUserName = false;

                // Do we have a username?
                if (info.Principal.HasClaim(c => c.Type == ClaimTypes.Name))
                {
                    // we do!
                    userName = info.Principal.FindFirstValue(ClaimTypes.Name);
                    hasUserName = true;
                }
                var user = new ApplicationUser { UserName = hasUserName ? userName : email, Email = email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return true;
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return false;
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
                var user = new ApplicationUser { UserName = Input.Email, Email = Input.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
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