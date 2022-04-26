using BoxTrackLabel.API.Models;

namespace BoxTrackLabel.API.Repositories
{
    public class OrderSettingsRepository: EfDataMatrixRepository<OrderSetting, BoxTrackDbContext>
    {
        private readonly BoxTrackDbContext _context;
        public OrderSettingsRepository(BoxTrackDbContext context) : base(context)
        {
            _context = context;
        }
        
    }
}