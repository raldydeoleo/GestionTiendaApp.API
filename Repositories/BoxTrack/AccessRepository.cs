using BoxTrackLabel.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxTrackLabel.API.Repositories
{
    public class AccessRepository
    {
        private readonly BoxTrackDbContext context;

        public AccessRepository(BoxTrackDbContext context)
        {
            this.context = context;
        }
        /// <summary>
        /// Agrega un nuevo registro de acceso del usuario
        /// </summary>
        public async Task Add(string token, string user, DateTime expiration)
        {
            var userAccess = await context.Accesses.Where(a => a.Usuario == user).FirstOrDefaultAsync(); 
            if(userAccess != null)
            {
                userAccess.Token = token;
                userAccess.FechaHoraAcceso = DateTime.Now;
                userAccess.Expiracion = expiration;
            }
            else
            {
                var access = new Access() { Usuario = user, Token = token, FechaHoraAcceso = DateTime.Now, Expiracion = expiration };
                context.Accesses.Add(access);
            }
            await context.SaveChangesAsync();
        }
        /// <summary>
        /// Obtiene el registro de acceso del usuario mediante su token
        /// </summary>
        public async Task<Access> GetAccessByToken(string token)
        {
            var access = await context.Accesses.Where(a => a.Token == token).FirstOrDefaultAsync();
            return access;
        }
        /// <summary>
        /// Actualiza el token del usuario cuando se realiza el proceso de tokenrefresh
        /// </summary>
        public async Task UpdateToken(Access access, string refreshedToken, DateTime expiration)
        {
            access.Token = refreshedToken;
            access.FechaHoraRefresh = DateTime.Now;
            access.Expiracion = expiration;
            context.Accesses.Update(access);
            await context.SaveChangesAsync();
        }
    }
}
