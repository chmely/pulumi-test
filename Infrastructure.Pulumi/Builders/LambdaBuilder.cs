using LetsGetChecked.Infrastructure.Misc;
using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.Lambda;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LetsGetChecked.Infrastructure.Builders
{
    internal class LambdaBuilder : ResourceBuilder<IInfrastructureBuilder, Function>, ILambdaBuilder
    {
        private readonly string _functionName;
        private string _handler;
        private string _streamTableName;
        private Amazon.Lambda.EventSourcePosition _streamSourcePosition;

        public LambdaBuilder(IInfrastructureBuilder parentBuilder, string functionName, Region region) : base(parentBuilder, region)
            => _functionName = functionName.NotNullOrWhiteSpace(nameof(functionName));

        public ILambdaBuilder WithHandler(string handler)
        {
            _handler = handler.NotNullOrWhiteSpace(nameof(handler));
            return this;
        }

        public ILambdaBuilder WithDynamoDbTrigger(string tableName, Amazon.Lambda.EventSourcePosition sourcePosition)
        {
            _streamTableName = tableName.NotNullOrWhiteSpace(nameof(tableName));
            _streamSourcePosition = sourcePosition;
            return this;
        }

        private static Pulumi.Aws.Iam.Role CreateRole(string roleName)
        {
            return new Pulumi.Aws.Iam.Role(
                roleName,
                new Pulumi.Aws.Iam.RoleArgs
                {
                    Name = roleName,
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
        }

        private static Pulumi.Aws.Iam.RolePolicyAttachment CreateRolePolicyAttachment(string name, Pulumi.Aws.Iam.Role lambdaRole)
        {
            return new Pulumi.Aws.Iam.RolePolicyAttachment(
                name,
                new Pulumi.Aws.Iam.RolePolicyAttachmentArgs
                {
                    Role = lambdaRole.Name,
                    PolicyArn = "arn:aws:iam::aws:policy/service-role/AWSLambdaBasicExecutionRole"
                });
        }

        public IInfrastructureBuilder EndLambda()
            => _parentBuilder;

        protected override Task<List<(string Name, CustomResource Resource)>> CreateResources()
        {
            var lambdaRole = CreateRole($"lambda-{_functionName}-execution-role");
            _ = CreateRolePolicyAttachment("$\"lambda-{_functionName}-execution-role-policy", lambdaRole);

            var bucketName = _region == Region.USEast1 ? "pch-appveyor-us" : "pch-appveyor";

            if (!string.IsNullOrWhiteSpace(_streamTableName))
            {
                var table = ResourceLookup.GetResource<Pulumi.Aws.DynamoDB.Table>(_streamTableName);
                _ = new EventSourceMapping(
                    $"{_streamTableName}_trigger",
                    new EventSourceMappingArgs
                    {
                        EventSourceArn = table.StreamArn,
                        FunctionName = _functionName,
                        StartingPosition = _streamSourcePosition.Value
                    },
                    ResourceOptions());
            }

            var lambda = new Function(
                _functionName,
                new FunctionArgs
                {
                    Name = _functionName,
                    Runtime = Amazon.Lambda.Runtime.Dotnet6.Value,
                    S3Bucket = bucketName,
                    S3Key = "dummy/dummy.zip",
                    Handler = _handler,
                    Role = lambdaRole.Arn
                });

            return ResultAsync((_functionName, lambda));
        }
    }
}
