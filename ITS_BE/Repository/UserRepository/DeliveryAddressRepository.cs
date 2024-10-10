using ITS_BE.Data;
using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using ITS_BE.Repository.ProductRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ITS_BE.Repository.UserRepository
{
    public class DeliveryAddressRepository : CommonRepository<DeliveryAddress>, IDeliveryAddressRepository
    {
        private readonly MyDbContext _context;
        public DeliveryAddressRepository(MyDbContext context) : base(context)
        {
            _context = context;
        }
    }
}