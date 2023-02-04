using MycoMgmt.Core.Models;
using MycoMgmt.Core.Models.DTO;
using Neo4j.Driver;

namespace MycoMgmt.Infrastructure.Repositories;

public interface IActionRepository
{
    public Task<List<IEntity>> SearchByName<T>(T model) where T : ModelBase;
    public Task<IEntity> GetByName<T>(T model) where T : ModelBase;
    public Task<GetNodeByIdDto> GetById<T>(T model) where T : ModelBase;
    public Task<IEnumerable<object>> GetAll<T>(T model, int skip, int limit) where T : ModelBase;
    public Task<List<IEntity>> Create<T>(T model) where T : ModelBase;
    public Task Delete<T>(T model) where T : ModelBase;
    public Task<List<IEntity>> Update<T>(T model) where T : ModelBase;
}