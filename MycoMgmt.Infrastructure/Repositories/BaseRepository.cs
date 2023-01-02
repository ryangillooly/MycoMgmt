using Neo4j.Driver;

namespace MycoMgmt.Infrastructure.Repositories;
public abstract class BaseRepository<ModelBase>
{
    public abstract Task<string> SearchByName(ModelBase model);
    public abstract Task<string> GetByName(ModelBase model);
    public abstract Task<string> GetById(ModelBase model);
    public abstract Task<string> GetAll(ModelBase model, int skip, int limit);
    public abstract Task<List<IEntity>> Create(ModelBase model);
    public abstract Task Delete(ModelBase model);
    public abstract Task<string> Update(ModelBase model);
}