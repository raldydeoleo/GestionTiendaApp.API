using BoxTrackLabel.API.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BoxTrackLabel.API.Repositories
{
    public interface IUserRepository
    {
        /// <summary>
        /// Recupera un usuario desde la base de datos mediante su nombre y clave
        /// </summary>
        /// <param name="userName">Nombre de usuario</param>
        /// <param name="clave">Clave de acceso</param>
        /// <returns>Usuario</returns>
        Task<User> FindUserByNameAndPasswordAsync(string userName, string clave);
        /// <summary>
        /// Recupera un usuario mediante su nombre
        /// </summary>
        /// <param name="userName">Nombre de usuario</param>
        /// <returns>Usuario</returns>
        Task<User> FindUserByNameAsync(string userName);
    }
    public class UserRepository: IUserRepository
    {
        private readonly MaestrosDbContext _context;
        public UserRepository(MaestrosDbContext context)
        {
            _context = context;
        }

        public async Task<User> FindUserByNameAndPasswordAsync(string userName, string clave)
        {
            var user = await _context.Users.Where(u=>u.NombreUsuario == userName && u.ClaveAcceso == clave).FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> FindUserByNameAsync(string userName)
        {
            var user = await _context.Users.Where(u => u.NombreUsuario == userName).FirstOrDefaultAsync();
            return user;
        }
    }
}
