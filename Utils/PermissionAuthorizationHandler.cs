using BoxTrackLabel.API.Repositories;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace BoxTrackLabel.API.Utils
{   /// <summary>
    /// Esta clase contiene el método que determina si el usuario que intenta acceder a un determinado recursos contiene los permisos para hacerlo
    /// </summary>
    internal class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRolPermissionRepository _rolPermissionRepository;

        public PermissionAuthorizationHandler(IUserRepository userRepository, IRolPermissionRepository rolPermissionRepository)
        {
            _userRepository = userRepository;
            _rolPermissionRepository = rolPermissionRepository;
        }
        /// <summary>
        /// Este método authoriza el acceso al recurso si dentro de los permisos del usuario se encuentra el permiso necesario.
        /// </summary>
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                return;
            }
            var uniqueName = context.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name");
            var user = await _userRepository.FindUserByNameAsync(uniqueName.Value);
            var userRolePermissions = await _rolPermissionRepository.FindUserPermissionsAsync(user.IdRol);

            var permissions = userRolePermissions.Where(p => "Permiso."+p.Permission.IdPermiso == requirement.Permission);

            if (permissions.Any())
            {
                context.Succeed(requirement);
                return;
            }
            
        }
    }
}
