using CargaAmbulatoria.EntityFramework.Enums;
using CargaAmbulatoria.EntityFramework.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CargaAmbulatoria.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private UserRoleEnum[] _roles = null;
        public AuthorizeAttribute()
        {

        }

        public AuthorizeAttribute(params UserRoleEnum[] roles)
        {
            _roles = roles;
        }


        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = (User)context.HttpContext.Items["User"];
            if (user == null || _roles != null && !_roles.Contains(user.Role))
            {
                // not logged in or without valid role
                context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
