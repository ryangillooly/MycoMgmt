using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Domain.Models;
using MycoMgmt.Infrastructure.Repositories;
using ILogger = Neo4j.Driver.ILogger;

namespace MycoMgmt.API.Controllers;

public class BaseController<T> : ControllerBase where T : BaseController<T>
{
    private IActionRepository? _repository;
    private ILogger<T>? _logger;

    protected ILogger<T> Logger => _logger ??= HttpContext.RequestServices.GetRequiredService<ILogger<T>>();
    protected IActionRepository Repository => _repository ??= HttpContext.RequestServices.GetRequiredService<IActionRepository>();
}