using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Amazon.DynamoDBv2;
using Amazon.Runtime;
using Amazon.DynamoDBv2.DataModel;
using MycoMgmt.Persistence.DynamoDB;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MycoMgmt.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CulturesController : ControllerBase
    {
        private static string tableName = "MycoMgmt";
        private static AmazonDynamoDBClient _client;

        public CulturesController(AmazonDynamoDBClient client) => _client = client;
        

        // GET: api/cultures
        [HttpGet]
        public IEnumerable<string> Get()
        {
            //var request = new QueryRequest()
            //client.QueryAsync();
            return new string[] { "value1", "value2" };
        }














        // GET api/cultures/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/cultures
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/cultures/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/cultures/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
