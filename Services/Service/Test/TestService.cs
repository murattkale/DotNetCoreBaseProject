public interface ITestService : IGenericRepo<Test> { }
public class TestService : GenericRepo<myDBContext, Test>, ITestService
{
    public TestService(myDBContext context, IBaseModel _IBaseModel) : base(context,_IBaseModel) { }
}

