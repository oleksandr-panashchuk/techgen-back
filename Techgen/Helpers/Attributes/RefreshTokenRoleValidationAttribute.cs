using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Localization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Techgen.Models.RequestModels;
using Techgen.Models.ResponseModels.Base;
using Techgen.ResourceLibrary;

namespace Techgen.Helpers.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class RefreshTokenRoleValidationAttribute : ActionFilterAttribute
    {
        private List<string> _roles;
        private IStringLocalizer<ErrorsResource> _errorsLocalizer;
        private ErrorResponseModel _errors;

        public RefreshTokenRoleValidationAttribute(string[] allowedRoles)
        {
            _roles = allowedRoles.ToList();
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (_errorsLocalizer == null)
                using (var scope = context.HttpContext.RequestServices.CreateScope())
                {
                    _errorsLocalizer = scope.ServiceProvider.GetRequiredService<IStringLocalizer<ErrorsResource>>();
                }

            var model = context.ActionArguments["model"] as RefreshTokenRequestModel;

            try
            {
                var decodedJwt = new JwtSecurityTokenHandler().ReadJwtToken(model.RefreshToken);

                if (decodedJwt.Claims.Any(x => x.Type == ClaimsIdentity.DefaultRoleClaimType && !_roles.Contains(x.Value)))
                {
                    _errors = new ErrorResponseModel(_errorsLocalizer);
                    context.Result = _errors.Error(System.Net.HttpStatusCode.Forbidden);

                    return;
                }

            } catch
            {
                _errors = new ErrorResponseModel(_errorsLocalizer);
                context.Result = _errors.BadRequest("refreshToken", "Refresh token is invalid");
            }

            return;
        }
    }
}
