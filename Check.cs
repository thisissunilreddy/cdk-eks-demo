using Amazon;
using Amazon.AutoScaling;
using Amazon.AutoScaling.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

// Set up the AWS credentials and region
var credentials = new BasicAWSCredentials(accessKey, secretKey);
var clientConfig = new AmazonDynamoDBConfig { RegionEndpoint = RegionEndpoint.USEast1 };

// Create a new DynamoDB client
var dynamoClient = new AmazonDynamoDBClient(credentials, clientConfig);

// Define the table name
var tableName = "my-table";

// Enable auto scaling for the table
var autoscalingClient = new AmazonAutoScalingClient(credentials, clientConfig);
var request = new PutScalingPolicyRequest
{
    PolicyName = "MyScalingPolicy",
    ServiceNamespace = "dynamodb",
    ResourceId = $"table/{tableName}",
    ScalableDimension = "dynamodb:table:ReadCapacityUnits",
    PolicyType = "TargetTrackingScaling",
    TargetTrackingScalingPolicyConfiguration = new TargetTrackingScalingPolicyConfiguration
    {
        TargetValue = 50,
        PredefinedMetricSpecification = new PredefinedMetricSpecification
        {
            PredefinedMetricType = "DynamoDBReadCapacityUtilization"
        }
    }
};


var response = await autoscalingClient.PutScalingPolicyAsync(request);
