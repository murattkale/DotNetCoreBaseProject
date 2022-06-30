public interface INationalityService : IGenericRepo<Nationality> { }
public class NationalityService : GenericRepo<myDBContext, Nationality>, INationalityService
{
    public NationalityService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }




}

