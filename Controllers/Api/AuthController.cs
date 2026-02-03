using AMZN.DTOs.Auth;
using AMZN.DTOs.Common;
using AMZN.Services.Auth;
using AMZN.Shared.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AMZN.Controllers.Api
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        
        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }


        // POST api/auth/register
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Register([FromBody] RegisterRequestDto request)
        {
            try
            {
                AuthResponseDto response = await _authService.RegisterAsync(request);
                return StatusCode(StatusCodes.Status201Created, response);
            }
            catch (AuthException ex)
            {
                return HandleAuthException(ex);
            }
        }


        // POST api/auth/login
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Login([FromBody]  LoginRequestDto request)
        {
            try
            {
                AuthResponseDto response = await _authService.LoginAsync(request);
                return Ok(response);

            }
            catch (AuthException ex)
            {
                return HandleAuthException(ex);
            }
        }


        // POST api/auth/refresh
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<ActionResult<AuthResponseDto>> Refresh([FromBody] RefreshRequestDto request)
        {
            try
            {
                AuthResponseDto response = await _authService.RefreshAsync(request);
                return Ok(response);
            }
            catch (AuthException ex)
            {
                return HandleAuthException(ex);
            }
        }


        private ActionResult HandleAuthException(AuthException ex)
        {
            int statusCode = MapAuthCodeToStatusCode(ex.Code);

            _logger.LogWarning(
                "Auth error: code={Code}, status={StatusCode}, traceId={TraceId}, message={Message}",
                ex.Code,
                statusCode,
                HttpContext.TraceIdentifier,
                ex.Message);

            // не светим внутренние детали наружу (DB/server errors)
            bool isServerError = ex.Code == AuthErrors.DatabaseError;

            string clientMessage = isServerError
                ? "Internal server error"
                : ex.Message;


            return StatusCode(statusCode, new ApiErrorResponse
            {
                Code = ex.Code,
                Message = clientMessage,
                TraceId = HttpContext.TraceIdentifier
            });
        }


        private static int MapAuthCodeToStatusCode(string code)
        {
            return code switch
            {
                AuthErrors.EmailTaken => StatusCodes.Status409Conflict,
                AuthErrors.InvalidCredentials => StatusCodes.Status401Unauthorized,
                AuthErrors.InvalidRefreshToken => StatusCodes.Status401Unauthorized,
                AuthErrors.DatabaseError => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status400BadRequest
            };
        }


    }
}
