public interface ICreditCardPrefixService : IGenericRepo<CreditCardPrefix> { }
public class CreditCardPrefixService : GenericRepo<myDBContext, CreditCardPrefix>, ICreditCardPrefixService
{
    public CreditCardPrefixService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

