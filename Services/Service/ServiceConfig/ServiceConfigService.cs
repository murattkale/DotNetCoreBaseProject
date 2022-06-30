public interface IServiceConfigService : IGenericRepo<ServiceConfig> { }
public class ServiceConfigService : GenericRepo<myDBContext, ServiceConfig>, IServiceConfigService
{
    public ServiceConfigService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

