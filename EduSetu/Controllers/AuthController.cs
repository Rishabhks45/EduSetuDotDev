using EduSetu.Application.Common.Helpers;
using EduSetu.Application.Features.Authentication;
using EduSetu.Application.Features.Authentication.Request;
using EduSetu.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

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
            new(ClaimTypes.Role, getUserResponse.Payload.Role ?? "")
        };

            // Create claims identity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            // Configure authentication properties based on RememberMe option
            AuthenticationProperties authProperties = new AuthenticationProperties
            {
                AllowRefresh = true,
                IsPersistent = false,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(30)
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





        /// <summary>
        /// Handles user logout with BroadcastChannel cross-tab logout support
        /// </summary>
        /// <returns>Redirect to login page</returns>
        [HttpGet("logout")]
        [AllowAnonymous]
        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/login");
        }
    }
}
