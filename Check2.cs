using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

var client = new AmazonDynamoDBClient();

var request = new CreateGlobalTableRequest
{
    GlobalTableName = "MyReplicaTable",
    ReplicationGroup = new List<Replica>
    {
        new Replica { RegionName = "us-west-2" },
        new Replica { RegionName = "eu-west-1" }
    }
};

var response = await client.CreateGlobalTableAsync(request);
