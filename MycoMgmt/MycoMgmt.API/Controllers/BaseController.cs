using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models;
using MycoMgmt.Infrastructure.Repositories;
using ILogger = Neo4j.Driver.ILogger;

namespace MycoMgmt.API.Controllers;

public class BaseController<T> : ControllerBase where T : ModelBase
{
    private ILogger<T>? _logger;
    private BaseRepository? _repository;

    protected ILogger<T> Logger => _logger ??= HttpContext.RequestServices.GetRequiredService<ILogger<T>>();
    protected BaseRepository Repository => _repository ??= HttpContext.RequestServices.GetRequiredService<BaseRepository>();
}