public interface ISpecService : IGenericRepo<Spec> { }
public class SpecService : GenericRepo<myDBContext, Spec>, ISpecService
{
    public SpecService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

