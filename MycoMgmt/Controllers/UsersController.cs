using Microsoft.AspNetCore.Mvc;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Amazon.Runtime;
using System.Collections.Generic;
using System;
using Amazon.Runtime.Internal.Transform;

namespace MycoMgmt.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private static string tableName = "MycoMgmt";
        private static AmazonDynamoDBClient _client;

        public UsersController(AmazonDynamoDBClient client) => _client = client;

        // GET: api/users
        [HttpGet]
        [Route("[controller]/{userEmail}")]
        public IActionResult Get(string userEmail)
        {
            var request = new QueryRequest
            {
                TableName = tableName,
                KeyConditionExpression = "UserEmail = :UserEmail",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {{
                    ":UserEmail",
                    new AttributeValue { S = userEmail } 
                 }}
            };

            var requestResponse = _client.QueryAsync(request);

            List<string> strings = null;

            foreach (Dictionary<string, AttributeValue> item in requestResponse.Result.Items)
            {
                // Process the result.
                strings.Add(item.Values.ToString());
            }

            return (IActionResult)strings;
        }
    }
}
