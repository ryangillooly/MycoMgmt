using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MycoMgmt.Core.Models.Mushrooms;
using MycoMgmt.Core.Services;

namespace MycoMgmt.Web.Controllers;

public class CultureController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IActionService _actionService;
    private const string url = "http://localhost:6002/culture";
    public string responseString;
    public List<string> strains;
    
    public CultureController(IHttpClientFactory clientFactory, IActionService actionService)
    {
        _clientFactory = clientFactory;
        _actionService = actionService;
    }
    
    
    public async Task OnGetAsync(Guid id)
    {
        var culture = await _actionService.GetById(new Culture  { Id = id });
    }
}