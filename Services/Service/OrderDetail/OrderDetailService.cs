public interface IOrderDetailService : IGenericRepo<OrderDetail> { }
public class OrderDetailService : GenericRepo<myDBContext, OrderDetail>, IOrderDetailService
{
    public OrderDetailService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

