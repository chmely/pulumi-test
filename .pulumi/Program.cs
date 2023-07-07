using Pulumi;
using System.Collections.Generic;
using System.Linq;

return await Deployment.RunAsync(() =>
{
   var table = new Pulumi.Aws.DynamoDB.Table(
      "integration_events",
      new Pulumi.Aws.DynamoDB.TableArgs
        {
            Attributes = new Pulumi.Aws.DynamoDB.Inputs.TableAttributeArgs[]
            {
               new Pulumi.Aws.DynamoDB.Inputs.TableAttributeArgs
               {
                Name = "id",
                Type = "S"
               }
            }.ToList(),
            HashKey = "id",
            StreamEnabled = true,
            StreamViewType = "NEW_AND_OLD_IMAGES",
            BillingMode = "PAY_PER_REQUEST",
        });

        // Create an IAM role for the Lambda function
        var lambdaRole = new Pulumi.Aws.Iam.Role("myLambdaRole", new Pulumi.Aws.Iam.RoleArgs
        {
            AssumeRolePolicy = @"{
                ""Version"": ""2012-10-17"",
                ""Statement"": [{
                    ""Action"": ""sts:AssumeRole"",
                    ""Principal"": {
                        ""Service"": ""lambda.amazonaws.com""
                    },
                    ""Effect"": ""Allow"",
                    ""Sid"": """"
                }]
            }"
        });

        // Attach the AWSLambdaBasicExecutionRole policy to the IAM role
        var lambdaRolePolicyAttachment = new Pulumi.Aws.Iam.RolePolicyAttachment("myLambdaRolePolicyAttachment", new Pulumi.Aws.Iam.RolePolicyAttachmentArgs
        {
            Role = lambdaRole.Name,
            PolicyArn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
        });

        // Create a Lambda function
        var lambda = new Pulumi.Aws.Lambda.Function("myLambdaFunction", new Pulumi.Aws.Lambda.FunctionArgs
        {
            Runtime = "dotnet6",
            S3Bucket = "dev-appveyor",
            S3Key = "dummy/dummy.zip",
            Handler = "LetsGetChecked.TreatmentPlans.KafkaExporter.Lambda::LetsGetChecked.TreatmentPlans.KafkaExporter.Lambda.Function::Handler",
            Role = lambdaRole.Arn
        });

        // Create a DynamoDB trigger for the Lambda function
        var trigger = new Pulumi.Aws.Lambda.EventSourceMapping(
         "myTrigger",
          new Pulumi.Aws.Lambda.EventSourceMappingArgs
        {
            EventSourceArn = table.StreamArn,
            FunctionName = lambda.Name,
            StartingPosition = "LATEST"
        });

        // Attach the AWSLambdaBasicExecutionRole policy to the IAM role
        var lambdaRolePolicyAttachment2 = new Pulumi.Aws.Iam.RolePolicyAttachment("myLambdaRole2", new Pulumi.Aws.Iam.RolePolicyAttachmentArgs
        {
            Role = lambdaRole.Name,
            PolicyArn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
        });
   

   // Export the name of the bucket
   return new Dictionary<string, object?>
   {
      ["lambda"] = lambda.Id,
      ["tableName"] = table.Id
   };
});
