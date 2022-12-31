﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models.Mushrooms;
using Neo4j.Driver;

namespace MycoMgmt.API.Repositories
{
    public interface IBulkRepository
    {
        Task<string> SearchByName(Bulk bulk);
        Task<string> GetByName(Bulk bulk);
        public Task<string> GetById(Bulk bulk);
        public Task<string> GetAll(Bulk bulk, int skip, int limit);
        public Task<List<IEntity>> Create(Bulk bulk);
        public Task Delete(Bulk bulk);
        public Task<string> Update(Bulk bulk);
    }
}