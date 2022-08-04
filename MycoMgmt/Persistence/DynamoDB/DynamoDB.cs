using Amazon.DynamoDBv2;

namespace MycoMgmt.Classes.DynamoDB
{
    public class DynamoDB
    {
        private AmazonDynamoDBClient _client;

        public DynamoDB(AmazonDynamoDBClient client) => this._client = client;
    }
}
