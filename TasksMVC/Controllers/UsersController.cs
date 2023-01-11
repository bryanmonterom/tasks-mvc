using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TasksMVC.Models;

namespace TasksMVC.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        public UsersController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();  
        }



        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new IdentityUser(){Email = model.Email, UserName = model.Email};
            var response =await _userManager.CreateAsync(user, password:model.Password);
            if (response.Succeeded)
            {

                await _signInManager.SignInAsync(user, isPersistent: true);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                foreach (var item in response.Errors)
                {
                    ModelState.AddModelError(String.Empty, item.Description);
                }    
            }
            return View(model);
        }

        [AllowAnonymous]
        public IActionResult Login(string message = null)
        {
            if (message is not null)
            {
                ViewBag.Message = message;
            }

            return View();
        }


        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "The User name or password provided is not valid");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(IdentityConstants.ApplicationScheme);
            return RedirectToAction("Index", "Home");
        }
        [AllowAnonymous]

        public async Task<IActionResult> RegisterExternalUser(string returnUrl = null, string remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            var message = "";

            if (remoteError is not null)
            {
                message = $"Error from external provider: {remoteError}";
                return RedirectToAction("Login", routeValues: new
                {
                    message
                });
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info is null)
            {
                message = "Error loading the information from external login provider";
                return RedirectToAction("Login", routeValues: new
                {
                    message
                });
            }

            var results = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, true,
                bypassTwoFactor: true);

            if (results.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            string email = "";
            if (info.Principal.HasClaim(a => a.Type == ClaimTypes.Email))
            {
                email = info.Principal.FindFirstValue(ClaimTypes.Email);
            }
            else
            {
                message = "Error reading email from the provided";
                return RedirectToAction("Login", routeValues: new
                {
                    message
                });
            }

            var user = new IdentityUser() { Email = email, UserName = email};
            var resultsCreateUser =await  _userManager.CreateAsync(user);
            if (!resultsCreateUser.Succeeded)
            {
                message = resultsCreateUser.Errors.First().Description;
                return RedirectToAction("Login", routeValues: new
                {
                    message
                });
            }

            var resultsAddLogin = await _userManager.AddLoginAsync(user, info);

            if (resultsAddLogin.Succeeded)
            {
                await _signInManager.SignInAsync(user, true, info.LoginProvider);
                return LocalRedirect(returnUrl);
            }

            message = "An error has ocurred adding the login";
            return RedirectToAction("Login", routeValues: new
            {
                message
            });
        }

        [AllowAnonymous]
        public ChallengeResult ExternalLogin(string provider, string returnUrl = null)
        {
            var urlRedirectrion = Url.Action("RegisterExternalUser", values: new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, urlRedirectrion);
            return new ChallengeResult(provider, properties);
        }
    }
}
