using BoxTrackLabel.API.Models;

namespace BoxTrackLabel.API.Repositories
{
    public class EmailAccountRepository: EfDataMatrixRepository<EmailAccount, BoxTrackDbContext>
    {
        private readonly BoxTrackDbContext _context;
        public EmailAccountRepository(BoxTrackDbContext context) : base(context)
        {
            _context = context;
        }
    }
}