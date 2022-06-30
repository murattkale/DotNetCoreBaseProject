public interface ISpecAttrService : IGenericRepo<SpecAttr> { }
public class SpecAttrService : GenericRepo<myDBContext, SpecAttr>, ISpecAttrService
{
    public SpecAttrService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

