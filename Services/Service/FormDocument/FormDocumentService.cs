public interface IFormDocumentService : IGenericRepo<FormDocument> { }
public class FormDocumentService : GenericRepo<myDBContext, FormDocument>, IFormDocumentService
{
    public FormDocumentService(myDBContext context, IBaseModel _IBaseModel) : base(context, _IBaseModel) { }
}

