public interface IFormTypeService : IGenericRepo<FormType> { }
public class FormTypeService : GenericRepo<myDBContext, FormType>, IFormTypeService
{
    public FormTypeService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

