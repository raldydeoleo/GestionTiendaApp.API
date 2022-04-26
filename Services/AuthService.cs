using BoxTrackLabel.API.Models;
using BoxTrackLabel.API.Repositories;
using BoxTrackLabel.API.Utils;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BoxTrackLabel.API.Services
{
    public interface IAuthService
    {
        Task<object> LoginAsync(string username, string clave);
        Task<object> RefreshToken(string expiredToken);
    }
    /// <summary>
    /// Esta clase maneja la autenticación mediante tokens y la actualización de tokens vencidos.
    /// </summary>
    public class AuthService: IAuthService
    {
        private readonly AppSettings _appSettings;
        private readonly IUserRepository _userRepository;
        private readonly IRolPermissionRepository _rolPermissionRepository;
        private readonly AccessRepository accessRepository;
        private List<RolPermission> userRolePermissions;

        public  AuthService(IOptions<AppSettings> appSettings, IUserRepository userRepository, IRolPermissionRepository rolPermissionRepository, AccessRepository accessRepository)
        {
            _appSettings = appSettings.Value;
            _userRepository = userRepository;
            _rolPermissionRepository = rolPermissionRepository;
            this.accessRepository = accessRepository;
        }
        /// <summary>
        /// Autentica un usuario mediante su nombre y clave
        /// </summary>
        /// <param name="username"></param>
        /// <param name="clave"></param>
        /// <returns>Token de acceso</returns>
        public async Task<object> LoginAsync(string username, string clave)
        {
            var user = await _userRepository.FindUserByNameAndPasswordAsync(username, clave);
            
            if (user != null)
            {
                userRolePermissions = await _rolPermissionRepository.FindUserPermissionsAsync(user.IdRol);
                return await BuildToken(user); 
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Devuelve un token de acceso para el usuario, incluye en el su rol y los permisos
        /// </summary>
        /// <param name="user"></param>
        /// <returns>Token de accesso</returns>
        private async Task<object> BuildToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.NombreUsuario),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("Rol", user.IdRol.ToString())
            };

            List<Claim> claimsPermissions = new List<Claim>();
            int i = 1;
            foreach (var p in userRolePermissions)
            {
                var claim = new Claim("Permission"+(i++), "Permiso."+p.Permission.IdPermiso);
                claimsPermissions.Add(claim);
            }
            claimsPermissions.OrderByDescending(c=>c.Type);
            claims = claims.Concat(claimsPermissions.ToArray()).ToArray();

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddHours(1);
            JwtSecurityToken token = new JwtSecurityToken(
               issuer: "laaurora.do",
               audience: "laaurora.do",
               claims: claims,
               expires: expiration,
               signingCredentials: creds);
            var tokenObject = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = expiration
            };
            await accessRepository.Add(tokenObject.token, user.NombreUsuario, tokenObject.expiration);
            return tokenObject;
        }
        /// <summary>
        /// Genera un nuevo token de acceso a partir del token expirado, utiliza este último para verificar que es una petición segura
        /// </summary>
        /// <param name="expiredToken"></param>
        /// <returns>Nuevo token de acceso</returns>
        public async Task<object> RefreshToken(string expiredToken)
        {
            var accessByToken = await accessRepository.GetAccessByToken(expiredToken);
            if(accessByToken != null)
            {
                var principal = GetPrincipalFromExpiredToken(expiredToken);
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expiration = DateTime.UtcNow.AddHours(1);
                JwtSecurityToken token = new JwtSecurityToken(
                   issuer: "laaurora.do",
                   audience: "laaurora.do",
                   claims: principal.Claims,
                   expires: expiration,
                   signingCredentials: creds);
                var tokenObject = new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = expiration
                };
                await accessRepository.UpdateToken(accessByToken, tokenObject.token, tokenObject.expiration);
                return tokenObject;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// Valida que el token de acceso vencido es legitimo y retorna sus parámetros principales
        /// </summary>
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true, 
                ValidateIssuer = true,
                ValidAudience = "laaurora.do",
                ValidIssuer = "laaurora.do",
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Secret)),
                ValidateLifetime = false 
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");

            return principal;
        }

    }
}
