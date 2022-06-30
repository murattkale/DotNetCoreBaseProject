using System;
using System.Linq;

public interface IUserService : IGenericRepo<User> {
    RModel<EnumModel> GetUserStatusType();
}
public class UserService : GenericRepo<myDBContext, User>, IUserService
{
    public UserService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }


    public RModel<EnumModel> GetUserStatusType()
    {
        var rModel = new RModel<EnumModel>();
        var list = Enum.GetValues(typeof(UserStatusType)).Cast<int>()
            .Select(x => new EnumModel { name = ((UserStatusType)x).ToStr(), value = x.ToString(), text = ((UserStatusType)x).ExGetDescription() }).ToList();
        rModel.ResultList = list;
        rModel.RType = RType.OK;
        return rModel;
    }

}

