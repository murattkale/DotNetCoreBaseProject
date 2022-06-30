public interface IUserRoleService : IGenericRepo<UserRole> { }
public class UserRoleService : GenericRepo<myDBContext, UserRole>, IUserRoleService
{
    public UserRoleService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

