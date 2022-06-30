public interface IOrderService : IGenericRepo<Order> { }
public class OrderService : GenericRepo<myDBContext, Order>, IOrderService
{
    public OrderService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

