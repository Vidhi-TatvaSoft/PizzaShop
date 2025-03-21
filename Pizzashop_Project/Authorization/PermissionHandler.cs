
using BLL.Interfaces;
using BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
namespace Pizzashop_Project.Authorization;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IRolesPermission _rolesPermission;
        private readonly IJWTTokenService _jWTService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PermissionHandler(IRolesPermission rolesPermission, IJWTTokenService jWTService, IHttpContextAccessor httpContextAccessor)
        {
            this._rolesPermission = rolesPermission;
            this._jWTService = jWTService;
            this._httpContextAccessor = httpContextAccessor;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var httpContext = _httpContextAccessor.HttpContext;
            var cookieSavedToken = httpContext.Request.Cookies["AuthToken"];
            if(cookieSavedToken == null)
            {
                context.Fail();
                return Task.CompletedTask;
            }
            var roleName = _jWTService.GetClaimValue(cookieSavedToken, "role");
            var permissionsData = _rolesPermission.permissionByRole(roleName);

            switch (requirement.Permission)
            {
                case "User.View":
                    if (permissionsData[0].Canview == true)
                        context.Succeed(requirement);
                    break;
                case "User.EditAdd":
                    if (permissionsData[0].Caneditadd == true)
                        context.Succeed(requirement);
                    break;
                case "User.Delete":
                    if (permissionsData[0].Candelete == true)
                        context.Succeed(requirement);
                    break;
                case "Role.View":
                    if (permissionsData[1].Canview == true)
                        context.Succeed(requirement);
                    break;
                case "Role.EditAdd":
                    if (permissionsData[1].Caneditadd == true)
                        context.Succeed(requirement);
                    break;
                case "Role.Delete":
                    if (permissionsData[1].Candelete == true)
                        context.Succeed(requirement);
                    break;
                case "Menu.View":
                    if (permissionsData[2].Canview == true)
                        context.Succeed(requirement);
                    break;
                case "Menu.EditAdd":
                    if (permissionsData[2].Caneditadd == true)
                        context.Succeed(requirement);
                    break;
                case "Menu.Delete":
                    if (permissionsData[2].Candelete == true)
                        context.Succeed(requirement);
                    break;
                case "TableSection.View":
                    if (permissionsData[3].Canview == true)
                        context.Succeed(requirement);
                    break;
                case "TableSection.EditAdd":
                    if (permissionsData[3].Caneditadd == true)
                        context.Succeed(requirement);
                    break;
                case "TableSection.Delete":
                    if (permissionsData[3].Candelete == true)
                        context.Succeed(requirement);
                    break;
                case "TaxFees.View":
                    if (permissionsData[4].Canview == true)
                        context.Succeed(requirement);
                    break;
                case "TaxFees.EditAdd":
                    if (permissionsData[4].Caneditadd == true)
                        context.Succeed(requirement);
                    break;
                case "TaxFees.Delete":
                    if (permissionsData[4].Candelete == true)
                        context.Succeed(requirement);
                    break;
                case "Orders.View":
                    if (permissionsData[5].Canview == true)
                        context.Succeed(requirement);
                    break;
                case "Orders.EditAdd":
                    if (permissionsData[5].Caneditadd == true)
                        context.Succeed(requirement);
                    break;
                case "Orders.Delete":
                    if (permissionsData[5].Candelete == true)
                        context.Succeed(requirement);
                    break;
                case "Customers.View":
                    if (permissionsData[6].Canview == true)
                        context.Succeed(requirement);
                    break;
                case "Customers.EditAdd":
                    if (permissionsData[6].Caneditadd == true)
                        context.Succeed(requirement);
                    break;
                case "Customers.Delete":
                    if (permissionsData[6].Candelete == true)
                        context.Succeed(requirement);
                    break;
                default:
                    break; 
            }
            return Task.CompletedTask;
        }
}