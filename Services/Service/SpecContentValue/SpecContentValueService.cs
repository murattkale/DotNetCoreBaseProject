public interface ISpecContentValueService : IGenericRepo<SpecContentValue> { }
public class SpecContentValueService : GenericRepo<myDBContext, SpecContentValue>, ISpecContentValueService
{
    public SpecContentValueService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

