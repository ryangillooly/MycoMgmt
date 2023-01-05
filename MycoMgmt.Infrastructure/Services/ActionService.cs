using MycoMgmt.Domain.Models;
using ErrorOr;

namespace MycoMgmt.Infrastructure.Services;

public class ActionService : IActionService
{
    private static readonly Dictionary<Guid, ModelBase> _model = new ();

    public void Create(ModelBase model)
    {
        _model.Add(model.Id, model);
    }

    public void Update(ModelBase model)
    {
        throw new NotImplementedException();
    }

    public void Delete(Guid id)
    {
        throw new NotImplementedException();
    }

    public ErrorOr<ModelBase> Get(Guid id)
    {
        throw new NotImplementedException();
    }
}