using BoxTrackLabel.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BoxTrackLabel.API.Repositories
{
    public class StorageRepository: EfCoreRepository<Storage, BoxTrackDbContext>
    {
        private readonly BoxTrackDbContext _context;
        public StorageRepository(BoxTrackDbContext context) : base(context)
        {
            _context = context;
        }
        
    }
}
