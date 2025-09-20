using EduSetu.Application.Common.Helpers;
using EduSetu.Application.Common.Interfaces;
using EduSetu.Application.Common.Settings;
using EduSetu.Application.Features.Authentication;
using EduSetu.Application.Features.Authentication.Request;
using EduSetu.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace EduSetu.Controllers
{
    /// <summary>
    /// Controller for handling essential authentication operations using CQRS pattern
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        public IMediator _mediator { get; }
        public IPasswordEncryptionService _PasswordEncryptionService { get; }
        public IOptions<EncryptionSettings> _EncryptionSettings { get; }

        public AuthController(IMediator mediator, IPasswordEncryptionService passwordEncryptionService, IOptions<EncryptionSettings> encryptionSettings)
        {
            _mediator = mediator;
            _PasswordEncryptionService = passwordEncryptionService;
            _EncryptionSettings = encryptionSettings;
        }

        #region SignIn User
        /// <summary>
        /// Completes the sign-in process using traditional MVC with HttpContext
        /// This is used for the Blazor authentication flow after CQRS validation
        /// </summary>
        /// <param name="token">Login token from CQRS validation</param>
        /// <param name="returnUrl">Optional return URL to redirect to after successful login</param>
        /// <returns>Redirect to dashboard or error page</returns>
        [HttpGet("signin")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(string token, string? returnUrl = null)
        {
            // Parse the login token to verify it's valid using CQRS
            ParseLoginTokenRequest parseTokenRequest = new ParseLoginTokenRequest(new ParseLoginTokenDto { Token = token });
            ParseLoginTokenResponse parseTokenResponse = await _mediator.Send(parseTokenRequest);
            if (parseTokenResponse.HasError || parseTokenResponse.Payload == null)
            {
                return Redirect("/login");
            }

            string email = parseTokenResponse.Payload.Email;
            bool rememberMe = parseTokenResponse.Payload.RememberMe;

            // Get user info from database using CQRS
            GetCurrentUserRequest getUserRequest = new GetCurrentUserRequest(new GetCurrentUserDto { Email = email });
            GetCurrentUserResponse getUserResponse = await _mediator.Send(getUserRequest);
            if (getUserResponse.HasError || getUserResponse.Payload == null)
            {
                return Redirect("/login");
            }

            // Create claims for the authenticated user
            List<Claim> claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, getUserResponse.Payload.UserId ?? ""),
                new(ClaimTypes.Name, getUserResponse.Payload.FullName ?? ""),
                new(ClaimTypes.Email, getUserResponse.Payload.Email ?? ""),
                new(ClaimTypes.GivenName, getUserResponse.Payload.FirstName ?? ""),
                new(ClaimTypes.Surname, getUserResponse.Payload.LastName ?? ""),
                new(ClaimTypes.Role, getUserResponse.Payload.Role ?? ""),
                new("ProfilePictureUrl", getUserResponse.Payload.ProfilePictureUrl ?? "")

            };

            // Create claims identity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Configure authentication properties based on RememberMe option
            AuthenticationProperties authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = rememberMe,
                ExpiresUtc = rememberMe
                    ? DateTimeOffset.UtcNow.AddDays(7)
                    : DateTimeOffset.UtcNow.AddMinutes(30)
            };

            if (rememberMe)
            {
                // Set remembered email cookie
                CookieOptions cookieOptions = new CookieOptions
                {
                    Expires = DateTime.Now.AddDays(7),
                    HttpOnly = true,
                    Secure = Request.IsHttps,
                    SameSite = SameSiteMode.Lax
                };
                Response.Cookies.Append("rememberedEmail", getUserResponse.Payload.Email ?? "", cookieOptions);
            }

            // Sign in the user
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            // Check for return URL first
            if (!string.IsNullOrEmpty(returnUrl))
            {
                string decodedReturnUrl = Uri.UnescapeDataString(returnUrl);
                // Validate that the return URL is a local URL for security
                if (Url.IsLocalUrl(decodedReturnUrl))
                {
                    return Redirect(decodedReturnUrl);
                }
            }

            // Redirect based on user role (default behavior)
            string redirectUrl;

            if (string.IsNullOrWhiteSpace(getUserResponse.Payload.Role) || !CommonHelper.TryParseEnum<UserRole>(getUserResponse.Payload.Role, out UserRole role))
            {
                redirectUrl = "/not-authorized";
            }
            else
            {
                redirectUrl = CommonHelper.GetDashboardUrlByRole(role);
            }

            return Redirect(redirectUrl);
        }
        #endregion


        #region Logout User

        /// <summary>
        /// Handles user logout with proper session cleanup
        /// </summary>
        /// <returns>Redirect to login page</returns>
        [HttpGet("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> LogOut()
        {
            // Clear authentication cookie
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            
            // Clear any remember me cookies
            if (Request.Cookies.ContainsKey("rememberedEmail"))
            {
                Response.Cookies.Delete("rememberedEmail");
            }
            
            // Clear the EduSetuAuth cookie explicitly
            Response.Cookies.Delete("EduSetuAuth", new CookieOptions
            {
                Path = "/",
                Domain = Request.Host.Host
            });

            // Redirect to login with logout event parameter
            return Redirect("/login?event=logout");
        }
        #endregion

        #region Google SignUp
        /// <summary>
        /// Initiates the Google login process
        /// </summary>
        /// <param name="returnUrl">Optional return URL to redirect to after successful login</param>
        /// <returns>Challenge for Google authentication</returns>
        [HttpGet("google-login")]
        [AllowAnonymous]
        public IActionResult GoogleLogin(string? returnUrl = null)
        {
            var redirectUrl = Url.Action("GoogleResponse", "Auth", new { returnUrl });
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }
        #endregion


        #region google Data Saving
        /// <summary>
        /// Handles the response from Google after authentication
        /// </summary>
        /// <param name="returnUrl">Optional return URL to redirect to after successful login</param>
        /// <returns>Redirect to dashboard or error page</returns>
        [HttpGet("GoogleResponse")]
        [AllowAnonymous]
        public async Task<IActionResult> GoogleResponse(string? returnUrl = null)
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);
            if (!authenticateResult.Succeeded)
                return Redirect("/login?error=Google authentication failed");

            // Extract user info from Google
            var email = authenticateResult.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = authenticateResult.Principal.Identity?.Name;
            var firstName = authenticateResult.Principal.FindFirst(ClaimTypes.GivenName)?.Value;
            var lastName = authenticateResult.Principal.FindFirst(ClaimTypes.Surname)?.Value;

            if (string.IsNullOrEmpty(email))
                return Redirect("/login?error=No email from Google");

            // Lookup user in your DB using CQRS

            //string tempPassword = CommonHelper.GenerateTemporaryPassword();
            string tempPassword = "Student@123";
            string hashedPassword = await _PasswordEncryptionService.EncryptPasswordAsync(tempPassword, _EncryptionSettings.Value.MasterKey);

            var getUserRequest = new GetCurrentUserRequest(new GetCurrentUserDto { Email = email });
            var getUserResponse = await _mediator.Send(getUserRequest);
            if (getUserResponse.HasError || getUserResponse.Payload == null)
            {
                // Register the user automatically
                var registerRequest = new RegisterUserRequest(new StudentDTOs
                {
                    Email = email,
                    FirstName = firstName ?? string.Empty,
                    LastName = lastName ?? string.Empty,
                    Role = UserRole.Student, // Default role
                   Password = hashedPassword // Random password for Google
                });
                var registerResponse = await _mediator.Send(registerRequest);
                if (registerResponse.HasError)
                    return Redirect("/login?error=Google registration failed");
                // Try to get the user again
                getUserResponse = await _mediator.Send(new GetCurrentUserRequest(new GetCurrentUserDto { Email = email }));
                if (getUserResponse.HasError || getUserResponse.Payload == null)
                    return Redirect("/login?error=Google registration failed");
            }

            // Create claims for the authenticated user
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, getUserResponse.Payload.UserId ?? ""),
                    new(ClaimTypes.Name, getUserResponse.Payload.FullName ?? name ?? ""),
                    new(ClaimTypes.Email, getUserResponse.Payload.Email ?? email),
                    new(ClaimTypes.GivenName, getUserResponse.Payload.FirstName ?? firstName ?? ""),
                    new(ClaimTypes.Surname, getUserResponse.Payload.LastName ?? lastName ?? ""),
                    new(ClaimTypes.Role, string.IsNullOrWhiteSpace(getUserResponse.Payload.Role) ? UserRole.Student.ToString() : getUserResponse.Payload.Role)
                };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                claimsPrincipal,
                authProperties);

            // Redirect to dashboard or returnUrl
            string dashboardUrl;
            string roleString = getUserResponse.Payload.Role;
            if (string.IsNullOrWhiteSpace(roleString))
                roleString = UserRole.Student.ToString();
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                dashboardUrl = returnUrl;
            }
            else if (!CommonHelper.TryParseEnum<UserRole>(roleString, out UserRole role))
            {
                dashboardUrl = "/not-authorized";
            }
            else
            {
                dashboardUrl = CommonHelper.GetDashboardUrlByRole(role);
            }
            return Redirect(dashboardUrl);
        }
        #endregion
    }
}
