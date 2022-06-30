public interface ICreditCardInstallmentService : IGenericRepo<CreditCardInstallment> { }
public class CreditCardInstallmentService : GenericRepo<myDBContext, CreditCardInstallment>, ICreditCardInstallmentService
{
    public CreditCardInstallmentService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

