using MycoMgmt.Domain.Models;
using ErrorOr;

namespace MycoMgmt.Infrastructure.Services;

public interface IActionService
{
    void Create(ModelBase model);
    void Update(ModelBase model);
    void Delete(Guid id);
    ErrorOr<ModelBase> Get(Guid id);
}