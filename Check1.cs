using Amazon;
using Amazon.AutoScaling;
using Amazon.AutoScaling.Model;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

// Set up the AWS SDK clients
var dynamoClient = new AmazonDynamoDBClient(RegionEndpoint.USWest2);
var autoscalingClient = new AmazonAutoScalingClient(RegionEndpoint.USWest2);

// Specify the name of the table and the initial read and write capacity units
var tableName = "MyTable";
var readCapacity = 10;
var writeCapacity = 10;

// Create the scalable targets for the table's read and write capacities
var readTarget = new ScalableTarget
{
    ServiceNamespace = "dynamodb",
    ResourceId = $"table/{tableName}",
    ScalableDimension = "dynamodb:table:ReadCapacityUnits",
    MinCapacity = 10,
    MaxCapacity = 100
};
var writeTarget = new ScalableTarget
{
    ServiceNamespace = "dynamodb",
    ResourceId = $"table/{tableName}",
    ScalableDimension = "dynamodb:table:WriteCapacityUnits",
    MinCapacity = 10,
    MaxCapacity = 100
};

// Register the scalable targets with Auto Scaling
await autoscalingClient.RegisterScalableTargetAsync(new RegisterScalableTargetRequest
{
    ServiceNamespace = readTarget.ServiceNamespace,
    ResourceId = readTarget.ResourceId,
    ScalableDimension = readTarget.ScalableDimension,
    MinCapacity = readTarget.MinCapacity,
    MaxCapacity = readTarget.MaxCapacity
});
await autoscalingClient.RegisterScalableTargetAsync(new RegisterScalableTargetRequest
{
    ServiceNamespace = writeTarget.ServiceNamespace,
    ResourceId = writeTarget.ResourceId,
    ScalableDimension = writeTarget.ScalableDimension,
    MinCapacity = writeTarget.MinCapacity,
    MaxCapacity = writeTarget.MaxCapacity
});

// Create the scaling policies for the table's read and write capacities
var readPolicy = new ScalingPolicy
{
    PolicyName = "ReadScalingPolicy",
    PolicyType = PolicyType.TargetTrackingScaling,
    TargetTrackingScalingPolicyConfiguration = new TargetTrackingScalingPolicyConfiguration
    {
        TargetValue = 70,
        ScaleInCooldown = 60,
        ScaleOutCooldown = 60,
        PredefinedMetricSpecification = new PredefinedMetricSpecification
        {
            PredefinedMetricType = PredefinedMetricType.DynamoDBReadCapacityUtilization
        }
    }
};
var writePolicy = new ScalingPolicy
{
    PolicyName = "WriteScalingPolicy",
    PolicyType = PolicyType.TargetTrackingScaling,
    TargetTrackingScalingPolicyConfiguration = new TargetTrackingScalingPolicyConfiguration
    {
        TargetValue = 70,
        ScaleInCooldown = 60,
        ScaleOutCooldown = 60,
        PredefinedMetricSpecification = new PredefinedMetricSpecification
        {
            PredefinedMetricType = PredefinedMetricType.DynamoDBWriteCapacityUtilization
        }
    }
};

// Put the scaling policies in effect for the table's read and write capacities
await autoscalingClient.PutScalingPolicyAsync(new PutScalingPolicyRequest
{
    PolicyName = readPolicy.PolicyName,
    ServiceNamespace = readTarget.ServiceNamespace,
    ResourceId = readTarget.ResourceId,
    ScalableDimension = readTarget.ScalableDimension,
    PolicyType = readPolicy.PolicyType,
    TargetTrackingScalingPolicyConfiguration = readPolicy.TargetTrackingScalingPolicyConfiguration
});
await autoscalingClient.PutScalingPolicyAsync(new PutScalingPolicyRequest
{
    PolicyName = writePolicy.PolicyName,
    ServiceNamespace = writeTarget.ServiceNamespace,
    ResourceId = writeTarget.ResourceId,
    ScalableDimension
