public interface IServiceConfigAuthService : IGenericRepo<ServiceConfigAuth> { }
public class ServiceConfigAuthService : GenericRepo<myDBContext, ServiceConfigAuth>, IServiceConfigAuthService
{
    public ServiceConfigAuthService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

