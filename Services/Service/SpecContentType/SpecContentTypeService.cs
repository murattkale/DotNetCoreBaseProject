public interface ISpecContentTypeService : IGenericRepo<SpecContentType> { }
public class SpecContentTypeService : GenericRepo<myDBContext, SpecContentType>, ISpecContentTypeService
{
    public SpecContentTypeService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

