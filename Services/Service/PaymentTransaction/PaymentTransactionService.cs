public interface IPaymentTransactionService : IGenericRepo<PaymentTransaction> { }
public class PaymentTransactionService : GenericRepo<myDBContext, PaymentTransaction>, IPaymentTransactionService
{
    public PaymentTransactionService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

