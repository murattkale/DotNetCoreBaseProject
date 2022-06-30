public interface IBankPrefixService : IGenericRepo<BankPrefix> { }
public class BankPrefixService : GenericRepo<myDBContext, BankPrefix>, IBankPrefixService
{
    public BankPrefixService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

