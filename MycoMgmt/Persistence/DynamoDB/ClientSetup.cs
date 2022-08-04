using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

namespace MycoMgmt.Persistence.DynamoDB
{
    public class ClientSetup
    {
        private AmazonDynamoDBClient _client;
        private DynamoDBContext _context;

        public void DynamoClient()
        {
            _client = new AmazonDynamoDBClient("accessid", "secretaccesskey",
                new AmazonDynamoDBConfig
                {
                    ServiceURL = "http://localhost:8000",
                    UseHttp = true
                }
            );

            _context = new DynamoDBContext(_client);
        }
    }
}
