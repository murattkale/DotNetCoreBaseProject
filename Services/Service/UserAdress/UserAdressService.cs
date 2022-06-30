public interface IUserAdressService : IGenericRepo<UserAdress> { }
public class UserAdressService : GenericRepo<myDBContext, UserAdress>, IUserAdressService
{
    public UserAdressService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

