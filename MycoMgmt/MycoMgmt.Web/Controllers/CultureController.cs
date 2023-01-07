using MycoMgmt.Domain.Models.Mushrooms;

namespace MycoMgmt.Web.Controllers;

using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using static Microsoft.AspNetCore.Mvc.Controller;

public class CultureController : Controller
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly IAction
    private const string url = "http://localhost:6002/culture";
    public string responseString;
    public List<string> strains;
    
    public CultureController(IHttpClientFactory clientFactory, A)
    {
        _clientFactory = clientFactory;
    }
    
    
    public async Task OnGetAsync(int id)
    {
        var culture = await _actionService.GetById(id);
    }
}