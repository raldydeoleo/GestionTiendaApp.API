using BoxTrackLabel.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxTrackLabel.API.Repositories
{
    public interface IRolPermissionRepository
    {
        /// <summary>
        /// Devuelve los permisos asociados a un determinado Rol
        /// </summary>
        /// <param name="rolId">Id del rol</param>
        /// <returns>Listado de permisos</returns>
        Task<List<RolPermission>> FindUserPermissionsAsync(int rolId);
    }
    public class RolPermissionRepository : IRolPermissionRepository
    {
        private readonly MaestrosDbContext _context;

        public RolPermissionRepository(MaestrosDbContext context)
        {
            _context = context;
        }
        public async Task<List<RolPermission>> FindUserPermissionsAsync(int rolId)
        {
            var rolPermissions = await _context.RolPermissions.Where(r => r.IdRol == rolId).Include(p=>p.Permission).ToListAsync();
            return rolPermissions;
        }
    }
}
