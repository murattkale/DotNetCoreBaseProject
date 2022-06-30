public interface IProductService : IGenericRepo<Product> { }
public class ProductService : GenericRepo<myDBContext, Product>, IProductService
{
    public ProductService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

