using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MycoMgmt.Domain.Models.Mushrooms;
using Newtonsoft.Json;

[IgnoreAntiforgeryToken]
public class CultureModel : PageModel
{
    private readonly IHttpClientFactory _clientFactory;
    private const string url = "http://localhost:6002/culture";
    public string responseString;
    public List<string> strains;
    
    public CultureModel(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    public void GoBack()
    {
        Redirect("/culture/list");
    }
    
    public async void OnGetAsync()
    {
        // Use the HttpClientFactory to create a new HttpClient
        var client = _clientFactory.CreateClient();
        
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        var response = await client.SendAsync(request);
        var responseResult = response.Content.ReadAsStringAsync().Result;
        var list = JsonConvert.DeserializeObject<List<string>>(responseResult);
        strains = list;
    }
    
    public async Task OnPostAsync()
    {
        // Use the HttpClientFactory to create a new HttpClient
        var client = _clientFactory.CreateClient();

        var paramString = "";

        // Get the values of the form inputs
        string? name         = Request.Form["name"];
        string? type         = Request.Form["type"];
        string? strain       = Request.Form["strain"];
        string? recipe       = Request.Form["recipe"];
        string? notes        = Request.Form["notes"];
        string? location     = Request.Form["location"];
        string? parent       = Request.Form["parent"];
        string? parentType   = Request.Form["parentType"];
        string? child        = Request.Form["child"];
        string? childType    = Request.Form["childType"];
        string? vendor       = Request.Form["vendor"];
        string? successful   = Request.Form["successful"];
        string? purchased    = Request.Form["purchased"];
        string? finished     = Request.Form["finished"];
        string? finishedOn   = Request.Form["finishedOn"];
        string? inoculatedOn = Request.Form["inoculatedOn"];
        string? inoculatedBy = Request.Form["inoculatedBy"];
        string? createdOn    = Request.Form["createdOn"];
        string? createdBy    = Request.Form["createdBy"];
        string? count        = Request.Form["count"];
        
        if (name         is not null and not "") paramString += $"name={name}&";
        if (type         is not null and not "") paramString += $"type={type}&";
        if (strain       is not null and not "") paramString += $"strain={strain}&";
        if (recipe       is not null and not "") paramString += $"recipe={recipe}&";
        if (notes        is not null and not "") paramString += $"notes={notes}&";
        if (location     is not null and not "") paramString += $"location={location}&";
        if (parent       is not null and not "") paramString += $"parent={parent}&";
        if (parentType   is not null and not "") paramString += $"parentType={parentType}&";
        if (child        is not null and not "") paramString += $"child={child}&";
        if (childType    is not null and not "") paramString += $"childType={childType}&";
        if (vendor       is not null and not "") paramString += $"vendor={vendor}&";
        if (successful   is not null and not "") paramString += $"successful=true&";
        if (finished     is not null and not "") paramString += $"finished=true&";
        if (purchased    is not null and not "") paramString += $"purchased=true&";
        if (finishedOn   is not null and not "") paramString += $"finishedOn={finishedOn}&";
        if (inoculatedOn is not null and not "") paramString += $"inoculatedOn={inoculatedOn}&";
        if (inoculatedBy is not null and not "") paramString += $"inoculatedBy={inoculatedBy}&";
        if (createdOn    is not null and not "") paramString += $"createdOn={createdOn}&";
        if (createdBy    is not null and not "") paramString += $"createdBy={createdBy}&";
        if (count        is not null and not "") paramString += $"count={count}&";

        paramString = paramString.TrimEnd('&');
        
        var request = new HttpRequestMessage(HttpMethod.Post, $"{url}?{paramString}");
        
        // Send a POST request to the API with the new object as the body
        var response = await client.SendAsync(request);
        var responseResult = response.Content.ReadAsStringAsync().Result;
        var nodeObj = JsonConvert.DeserializeObject<dynamic>(responseResult);
        
        var outputString = $"The following nodes were created \n"; 
        
        foreach (var item in nodeObj)
        {
            outputString += $"\n {item["Name"]} ({item["ElementId"]})"; 
        }

        responseString = outputString;
        
        // Check the status code of the response
        if (response.IsSuccessStatusCode)
        {
            // The API call was successful, display a message to the user
            TempData["Message"] = "Successfully created new object.";
        }
        else
        {
            // The API call was not successful, display an error message to the user
            TempData["Message"] = "An error occurred while creating the new object.";
        }
    }
}