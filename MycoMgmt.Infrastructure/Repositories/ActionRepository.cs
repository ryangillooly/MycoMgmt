using AutoMapper;
using Microsoft.Extensions.Logging;
using MycoMgmt.Domain.Models;
using MycoMgmt.Domain.Models.Mushrooms;
using MycoMgmt.Domain.Models.UserManagement;
using MycoMgmt.Infrastructure.DataStores.Neo4J;
using MycoMgmt.Infrastructure.ServiceErrors;
using Neo4j.Driver;
using Newtonsoft.Json;
using ILogger = Neo4j.Driver.ILogger;

#pragma warning disable CS8604

// ReSharper disable once CheckNamespace
namespace MycoMgmt.Infrastructure.Repositories
{
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
        
        public async Task<List<IEntity>> SearchByName<T>(T model) where T : ModelBase
        {
            var nodeList = await _neo4JDataAccess.ExecuteReadListAsync(model.SearchByNameQuery(), "x");
            var result = _mapper.Map<List<IEntity>>(nodeList);
            return result;
        }

        public async Task Delete<T>(T model) where T : ModelBase => await _neo4JDataAccess.ExecuteWriteTransactionAsync<IEntity>(model.Delete());
        public async Task<IEntity> GetByName<T>(T model) where T : ModelBase => await _neo4JDataAccess.ExecuteReadScalarAsync<IEntity>(model.GetByNameQuery());
        public async Task<ModelBase> GetById<T>(T model) where T : ModelBase => await _neo4JDataAccess.ExecuteReadScalarAsync<Culture>(model.GetByIdQuery());
    
        public async Task<IEnumerable<object>> GetAll<T>(T model, int skip, int limit) where T : ModelBase
        {
            var nodeList = await _neo4JDataAccess.ExecuteReadListAsync(model.GetAllQuery(skip, limit), "result");
           // var result = _mapper.Map<List<IEntity>>(nodeList);
            return nodeList;
        }
    }
}