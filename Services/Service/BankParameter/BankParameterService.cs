public interface IBankParameterService : IGenericRepo<BankParameter> { }
public class BankParameterService : GenericRepo<myDBContext, BankParameter>, IBankParameterService
{
    public BankParameterService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

