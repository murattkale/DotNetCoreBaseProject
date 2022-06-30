public interface IBankService : IGenericRepo<Bank> { }
public class BankService : GenericRepo<myDBContext, Bank>, IBankService
{
    public BankService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }




}

