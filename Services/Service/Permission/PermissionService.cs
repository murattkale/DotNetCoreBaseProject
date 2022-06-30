public interface IPermissionService : IGenericRepo<Permission> { }
public class PermissionService : GenericRepo<myDBContext, Permission>, IPermissionService
{
    public PermissionService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

