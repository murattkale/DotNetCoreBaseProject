public interface IDistrictService : IGenericRepo<District> { }
public class DistrictService : GenericRepo<myDBContext, District>, IDistrictService
{
    public DistrictService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }


}

