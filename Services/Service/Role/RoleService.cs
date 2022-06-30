public interface IRoleService : IGenericRepo<Role> { }
public class RoleService : GenericRepo<myDBContext, Role>, IRoleService
{
    public RoleService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

