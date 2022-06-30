public interface IFormsService : IGenericRepo<Forms> { }
public class FormsService : GenericRepo<myDBContext, Forms>, IFormsService
{
    public FormsService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

