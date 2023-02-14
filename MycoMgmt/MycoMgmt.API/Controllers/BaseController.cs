using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Infrastructure.Repositories;
using MycoMgmt.Core.Services;

namespace MycoMgmt.API.Controllers;

public class BaseController<T> : ControllerBase where T : BaseController<T>
{
    private IActionService?    _actionService;
    private IActionRepository? _repository;
    private ILogger<T>?        _logger;
    private IMapper?           _mapper;

    protected ILogger<T> Logger => _logger ??= HttpContext.RequestServices.GetRequiredService<ILogger<T>>();
    protected IActionRepository Repository => _repository ??= HttpContext.RequestServices.GetRequiredService<IActionRepository>();
    protected IActionService ActionService => _actionService ??= HttpContext.RequestServices.GetRequiredService<IActionService>();
    protected IMapper Mapper => _mapper ??= HttpContext.RequestServices.GetRequiredService<IMapper>();
}