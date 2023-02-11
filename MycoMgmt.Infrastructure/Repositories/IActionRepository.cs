using MycoMgmt.Core.Models;
using MycoMgmt.Core.Models.DTO;
using Neo4j.Driver;

namespace MycoMgmt.Infrastructure.Repositories;

public interface IActionRepository
{
    public Task<GetNodeDto> GetByName<T>(T model) where T : ModelBase;
    public Task<GetNodeDto> GetById<T>(T model) where T : ModelBase;
    public Task<IEnumerable<GetNodeDto>> GetAll<T>(T model, int skip, int limit) where T : ModelBase;
    public Task<IEnumerable<GetNodeDto>> SearchByName<T>(T model, int skip, int limit) where T : ModelBase;
    public Task<List<IEntity>> Create<T>(T model) where T : ModelBase;
    public Task Delete<T>(T model) where T : ModelBase;
    public Task<List<IEntity>> Update<T>(T model) where T : ModelBase;
}