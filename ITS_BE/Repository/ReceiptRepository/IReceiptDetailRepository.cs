using ITS_BE.Models;
using ITS_BE.Repository.CommonRepository;
using System.Linq.Expressions;

namespace ITS_BE.Repository.ReceiptRepository
{
    public interface IReceiptDetailRepository: ICommonRepository<ReceiptDetail>
    {
        //Task<ReceiptDetail?> SingleOrDefaultAsyncInclude(Expression<Func<ReceiptDetail, bool>> expression);
    }
}
