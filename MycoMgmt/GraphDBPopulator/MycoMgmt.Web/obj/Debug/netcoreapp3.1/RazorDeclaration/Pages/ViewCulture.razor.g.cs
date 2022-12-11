// <auto-generated/>
#pragma warning disable 1591
#pragma warning disable 0414
#pragma warning disable 0649
#pragma warning disable 0169

namespace MycoMgmt.Web.Pages
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Components;
#nullable restore
#line 1 "C:\Users\ryang\Documents\GitHub\MycoMgmt\MycoMgmt\GraphDBPopulator\MycoMgmt.Web\_Imports.razor"
using System.Net.Http;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\ryang\Documents\GitHub\MycoMgmt\MycoMgmt\GraphDBPopulator\MycoMgmt.Web\_Imports.razor"
using Microsoft.AspNetCore.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 3 "C:\Users\ryang\Documents\GitHub\MycoMgmt\MycoMgmt\GraphDBPopulator\MycoMgmt.Web\_Imports.razor"
using Microsoft.AspNetCore.Components.Authorization;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\ryang\Documents\GitHub\MycoMgmt\MycoMgmt\GraphDBPopulator\MycoMgmt.Web\_Imports.razor"
using Microsoft.AspNetCore.Components.Forms;

#line default
#line hidden
#nullable disable
#nullable restore
#line 5 "C:\Users\ryang\Documents\GitHub\MycoMgmt\MycoMgmt\GraphDBPopulator\MycoMgmt.Web\_Imports.razor"
using Microsoft.AspNetCore.Components.Routing;

#line default
#line hidden
#nullable disable
#nullable restore
#line 6 "C:\Users\ryang\Documents\GitHub\MycoMgmt\MycoMgmt\GraphDBPopulator\MycoMgmt.Web\_Imports.razor"
using Microsoft.AspNetCore.Components.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 7 "C:\Users\ryang\Documents\GitHub\MycoMgmt\MycoMgmt\GraphDBPopulator\MycoMgmt.Web\_Imports.razor"
using Microsoft.JSInterop;

#line default
#line hidden
#nullable disable
#nullable restore
#line 8 "C:\Users\ryang\Documents\GitHub\MycoMgmt\MycoMgmt\GraphDBPopulator\MycoMgmt.Web\_Imports.razor"
using MycoMgmt.Web;

#line default
#line hidden
#nullable disable
#nullable restore
#line 9 "C:\Users\ryang\Documents\GitHub\MycoMgmt\MycoMgmt\GraphDBPopulator\MycoMgmt.Web\_Imports.razor"
using MycoMgmt.Web.Shared;

#line default
#line hidden
#nullable disable
#nullable restore
#line 2 "C:\Users\ryang\Documents\GitHub\MycoMgmt\MycoMgmt\GraphDBPopulator\MycoMgmt.Web\Pages\ViewCulture.razor"
using Neo4j.Driver;

#line default
#line hidden
#nullable disable
#nullable restore
#line 4 "C:\Users\ryang\Documents\GitHub\MycoMgmt\MycoMgmt\GraphDBPopulator\MycoMgmt.Web\Pages\ViewCulture.razor"
using MycoMgmt.API.Models;

#line default
#line hidden
#nullable disable
    [global::Microsoft.AspNetCore.Components.RouteAttribute("/culture/{name}")]
    public partial class ViewCulture : global::Microsoft.AspNetCore.Components.ComponentBase
    {
        #pragma warning disable 1998
        protected override void BuildRenderTree(global::Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder)
        {
        }
        #pragma warning restore 1998
#nullable restore
#line 9 "C:\Users\ryang\Documents\GitHub\MycoMgmt\MycoMgmt\GraphDBPopulator\MycoMgmt.Web\Pages\ViewCulture.razor"
 
    // Define a RenderFragment<T> property to hold the component
    public RenderFragment<List<string>> MyComponent { get; set; }

    private List<string> MyList { get; set; } = new List<string>();
    
    [Parameter]
    public string name { get; set; }
    
    protected override async Task OnInitializedAsync()
    {
        MyComponent = (List<string> list) =>
        {
            return builder =>
            {
                foreach (var text in list)
                {
                     // Add the <p> tag to the component
                    builder.OpenElement(0, "p");
                    builder.AddContent(1, text);
                    builder.CloseElement();    
                    
                    // Add the <p> tag to the component
                    builder.OpenElement(0, "p");
                    builder.AddContent(1, text);
                    builder.CloseElement();    
                }
            };
        };
        
        var neo4jDriver = GraphDatabase.Driver("neo4j://localhost:7687", AuthTokens.Basic("rg", "rg"));

            // Use a try/catch block to handle any exceptions that may be thrown
        try
        {
                // Open a new session with the Neo4j database
            using (var session = neo4jDriver.AsyncSession(o => o.WithDatabase("mycomgmt")))
            {
             // Execute a Cypher query asynchronously using the Session.RunAsync() method
                var result = await session.RunAsync($"MATCH (c:Culture {{ Name: '{name}' }}) RETURN c");
       
                var records = await result.ToListAsync();
                // Process the results of the query
                var hasNext = await result.FetchAsync();

                foreach (var record in records)
                {
                    var obj = record["c"].As<INode>();
                    var culture = new Culture()
                    {
                        Name     = obj.Properties["Name"].ToString(),
                        Type     = Enum.Parse<CultureTypes>(obj.Properties["Type"].ToString()),
                        Location = Enum.Parse<Locations>(obj.Properties["Location"].ToString()),
                        Strain   = Enum.Parse<Strain>(obj.Properties["Strain"].ToString())
                    };
                    
                // Access the values of the returned record
                    var node = record["c"].As<INode>();
                    Console.WriteLine(node.Properties["Name"]);
                    MyList.Add(node.Properties["Name"].ToString());
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
        finally
        {
            // Dispose of the driver when finished
            neo4jDriver.Dispose();
        }
    }

#line default
#line hidden
#nullable disable
        [global::Microsoft.AspNetCore.Components.InjectAttribute] private NavigationManager NavigationManager { get; set; }
    }
}
#pragma warning restore 1591
