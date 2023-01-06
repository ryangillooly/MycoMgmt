using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MycoMgmt.Domain.Models;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Infrastructure.Services;
using ILogger = Neo4j.Driver.ILogger;

namespace MycoMgmt.API.Controllers;

public class BaseController<T> : ControllerBase where T : BaseController<T>
{
    private IActionService? _actionService;
    private IActionRepository? _repository;
    private ILogger<T>? _logger;

    protected ILogger<T> Logger => _logger ??= HttpContext.RequestServices.GetRequiredService<ILogger<T>>();
    protected IActionRepository Repository => _repository ??= HttpContext.RequestServices.GetRequiredService<IActionRepository>();
    protected IActionService ActionService => _actionService ??= HttpContext.RequestServices.GetRequiredService<IActionService>();
}