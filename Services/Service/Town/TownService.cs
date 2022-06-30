public interface ITownService : IGenericRepo<Town> { }
public class TownService : GenericRepo<myDBContext, Town>, ITownService
{
    public TownService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }


}

