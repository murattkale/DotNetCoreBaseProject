using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

public interface ICreditCardService : IGenericRepo<CreditCard> {

    RModel<CreditCard> GetCreditCardByPrefix(string prefix, bool includeInstallments = false);

}
public class CreditCardService : GenericRepo<myDBContext, CreditCard>, ICreditCardService
{
    public CreditCardService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }


    public RModel<CreditCard> GetCreditCardByPrefix(string prefix, bool includeInstallments = false)
    {
        prefix = prefix.Trim();
        var query = Get(x => x.Prefixes.Any(cp => cp.Prefix.Equals(prefix)),true,false,o=>o.Installments);
        //if (query!=null && query.ResultRow != null && includeInstallments)
        //    query.Result.Include(x => x.Installments);
        return query;
    }


}

