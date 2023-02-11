using AutoMapper;
using Microsoft.Extensions.Logging;
using MycoMgmt.Core.Models;
using MycoMgmt.Core.Models.DTO;
using MycoMgmt.Infrastructure.DataStores.Neo4J;
using Neo4j.Driver;

namespace MycoMgmt.Infrastructure.Repositories;

public class ActionRepository : IActionRepository
{
    private readonly INeo4JDataAccess _neo4JDataAccess;
    private readonly ILogger<ActionRepository> _logger;
    private readonly IMapper _mapper;

    public ActionRepository(INeo4JDataAccess dataAccess, ILogger<ActionRepository> logger, IMapper mapper)
    {
        _neo4JDataAccess = dataAccess;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<List<IEntity>> Create<T>(T model) where T : ModelBase
    {
        var queryList = model.CreateQueryList();
        var result =  await _neo4JDataAccess.RunTransaction(queryList);
        return result;
    }
    public async Task<List<IEntity>> Update<T>(T model) where T : ModelBase
    {
        var queryList = model.UpdateQueryList();
        var result = await _neo4JDataAccess.RunTransaction(queryList);
        return result;
    }
    public async Task Delete<T>(T model) where T : ModelBase => await _neo4JDataAccess.ExecuteWriteTransactionAsync<IEntity>(model.Delete());
    public async Task<GetNodeDto> GetByName<T>(T model) where T : ModelBase => await _neo4JDataAccess.ExecuteReadScalarAsync<GetNodeDto>(model.GetByNameQuery());
    public async Task<GetNodeDto> GetById<T>(T model) where T : ModelBase => await _neo4JDataAccess.ExecuteReadScalarAsync<GetNodeDto>(model.GetByIdQuery());
    public async Task<IEnumerable<GetNodeDto>> GetAll<T>(T model, int skip, int limit) where T : ModelBase => await _neo4JDataAccess.ExecuteReadListAsync<GetNodeDto>(model.GetAllQuery(skip, limit), "result");
    public async Task<IEnumerable<GetNodeDto>> SearchByName<T>(T model, int skip, int limit) where T : ModelBase => await _neo4JDataAccess.ExecuteReadListAsync<GetNodeDto>(model.SearchByNameQuery(skip, limit), "result");
}
