using CargaAmbulatoria.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace CargaAmbulatoria.Services.Middlewares
{
    public class JwtMiddleware
    {
        #region MyRegion

        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        #endregion

        public JwtMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        #region Private Methods
        /// <summary>
        /// Attach user on Context, add Token with expiration datetime.
        /// </summary>
        /// <param name="context">context of request</param>
        /// <param name="authenticateService">default service</param>
        /// <param name="token">token</param>
        private void AttachUserToContext(HttpContext context, IAuthenticateService authenticateService, string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = jwtToken.Claims.First(x => x.Type == "UserId").Value;

                // attach user to context on successful jwt validation
                context.Items["User"] = authenticateService.GetById(userId);
            }
            catch
            {
                // do nothing if jwt validation fails
                // user is not attached to context so request won't have access to secure routes
            }
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Invoke validation as default.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="authenticateService"></param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context, IAuthenticateService authenticateService)
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
                AttachUserToContext(context, authenticateService, token);

            await _next(context);
        }
        #endregion
    }
}
