using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

// Set up the Amazon DynamoDB client
var client = new AmazonDynamoDBClient();

// Define the table schema
var tableRequest = new CreateTableRequest
{
    TableName = "myTable",
    KeySchema = new List<KeySchemaElement>
    {
        new KeySchemaElement("id", KeyType.HASH)
    },
    AttributeDefinitions = new List<AttributeDefinition>
    {
        new AttributeDefinition("id", ScalarAttributeType.S)
    },
    BillingMode = BillingMode.PAY_PER_REQUEST,
    GlobalSecondaryIndexes = new List<GlobalSecondaryIndex>
    {
        new GlobalSecondaryIndex
        {
            IndexName = "myIndex",
            KeySchema = new List<KeySchemaElement>
            {
                new KeySchemaElement("name", KeyType.HASH)
            },
            Projection = new Projection
            {
                ProjectionType = ProjectionType.ALL
            },
            ProvisionedThroughput = new ProvisionedThroughput
            {
                ReadCapacityUnits = 5,
                WriteCapacityUnits = 5
            }
        }
    },
    ProvisionedThroughput = new ProvisionedThroughput
    {
        ReadCapacityUnits = 5,
        WriteCapacityUnits = 5
    }
};

// Add autoscaling for read and write capacity
tableRequest.SSESpecification = new SSESpecification
{
    Enabled = true
};

tableRequest.GlobalSecondaryIndexes[0].ProvisionedThroughput.AutoScalingSettings = new AutoScalingSettings
{
    MinimumUnits = 1,
    MaximumUnits = 100,
    AutoScalingDisabled = false
};

tableRequest.GlobalSecondaryIndexes[0].ProvisionedThroughput.AutoScalingSettings.ScaleInCooldown = 60;
tableRequest.GlobalSecondaryIndexes[0].ProvisionedThroughput.AutoScalingSettings.ScaleOutCooldown = 60;

tableRequest.ProvisionedThroughput.AutoScalingSettings = new AutoScalingSettings
{
    MinimumUnits = 1,
    MaximumUnits = 100,
    AutoScalingDisabled = false
};

tableRequest.ProvisionedThroughput.AutoScalingSettings.ScaleInCooldown = 60;
tableRequest.ProvisionedThroughput.AutoScalingSettings.ScaleOutCooldown = 60;

// Create the table
var response = client.CreateTableAsync(tableRequest).Result;

// Wait for the table to be created
client.WaitUntilTableExistsAsync("myTable").Wait();

// Create a global table
var globalTableRequest = new CreateGlobalTableRequest
{
    GlobalTableName = "myGlobalTable",
    ReplicationGroup = new List<Replica>
    {
        new Replica
        {
            RegionName = RegionEndpoint.USWest1.SystemName
        },
        new Replica
        {
            RegionName = RegionEndpoint.EUWest1.SystemName
        }
    }
};

var globalTableResponse = client.CreateGlobalTableAsync(globalTableRequest).Result;
