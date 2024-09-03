using Microsoft.AspNetCore.Mvc;
using AuthDemo.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;


namespace AuthDemo.Controllers;

public class AccountController : Controller
{
    // Mocked user data
    private const string MockedUsername = "demo";
    private const string MockedPassword = "pass"; // Note: NEVER hard-code passwords in real applications.

    public IActionResult Login()
    {
        return View();
    }

    [Authorize] // This attribute ensures that only authenticated users can access this action.
    public IActionResult SecretInfo()
    {
        return View();
    }



    [HttpPost]
    [ValidateAntiForgeryToken] // This ensures that the form is submitted with a valid anti-forgery token to prevent CSRF attacks.
    public async Task<IActionResult> LoginAsync(LoginViewModel model)
    {
        // Check model validators
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Mocked user verification
        if (model.Username == MockedUsername && model.Password == MockedPassword)
        {
            // Set up the session/cookie for the authenticated user.
            var claims = new[] { new Claim(ClaimTypes.Name, model.Username) };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            // Normally, here you'd set up the session/cookie for the authenticated user.
            return RedirectToAction("Index", "Home"); // Redirect to a secure area of your application.
        }

        ModelState.AddModelError(string.Empty, "Invalid login attempt."); // Generic error message for security reasons.
        return View(model);
    }


    [Authorize]
    public IActionResult Logout()
    {
        return SignOut(
            new AuthenticationProperties
            {
                RedirectUri = Url.Action("Index", "Home")
            },
            CookieAuthenticationDefaults.AuthenticationScheme);
    }


    public IActionResult GoogleLogin()
    {
        var authProperties = new AuthenticationProperties 
        {
            RedirectUri = Url.Action("GoogleLoginCallback", "Account")
        };
        return Challenge(authProperties, GoogleDefaults.AuthenticationScheme);
    }

    public async Task<IActionResult> GoogleLoginCallbackAsync()
    {
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (!result.Succeeded)
        {
            // Handle failure: return to the login page, show an error, etc.
            return RedirectToAction("Login");
        }

        // Here, you could fetch information from result.Principal to store in your database, 
        // or to find an existing user.

        return RedirectToAction("Index", "Home");
    }


}