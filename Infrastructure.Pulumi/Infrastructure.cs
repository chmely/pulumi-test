using Amazon.Lambda.Model;
using Amazon.Runtime;
using LetsGetChecked.Infrastructure.Builders;
using Pulumi;
using Pulumi.Aws;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LetsGetChecked.Infrastructure
{
    public static class Infrastructure
    {
        private static readonly Builder builder = new();
        public static IInfrastructureBuilder Builder => builder;
    }

    class Builder : IInfrastructureBuilder
    {
        private Region _region = Region.EUWest1;
        private readonly List<IResourceBuilder> _builders = new ();

        public Task Create()
        {
            Debugger.Launch();
            Debugger.Break();

            return Deployment.RunAsync(async () =>
            {
                foreach (var builder in _builders)
                    await builder.Create();
            });
        }

        public IDynamoDbBuilder DynamoDb(string tableName)
        {
            var ddbBuilder = new DynamoDbBuilder(this, tableName, _region);
            _builders.Add(ddbBuilder);
            return ddbBuilder;
        }
            

        public IInfrastructureBuilder KafkaTopic(string topicName)
        {
            var kafkaTopicBuilder = new KafkaTopicBuilder(this, _region, topicName);
            _builders.Add(kafkaTopicBuilder);
            return this;
        }

        public ILambdaBuilder Lambda(string lambdaName)
        {
            var lambdaBuilder = new LambdaBuilder(this, lambdaName, _region);
            _builders.Add(lambdaBuilder);
            return lambdaBuilder;
        }

        public IInfrastructureBuilder SnsTopic(string topicName)
        {
            throw new NotImplementedException();
        }

        public IInfrastructureBuilder Sqs(string queueName)
        {
            var sqsBuilder = new SqsBuilder(this, _region, queueName);
            _builders.Add(sqsBuilder);
            return this;
        }

        public IInfrastructureBuilder ForRegion(Region region)
        {
            if (region != Region.EUWest1 && region != Region.USEast1)
                throw new ArgumentOutOfRangeException(nameof(region), region, $"Supported regions are {Region.EUWest1} and {Region.USEast1} only.");

            _region = region;

            return this;
        }
    }

    public interface IResourceBuilder
    {
        Task Create();
    }

    internal interface IResourceArgBuilder<TResourceArg>
    {
        Task<TResourceArg> Create();
    }

    public interface IInfrastructureBuilder: IResourceBuilder
    {
        IDynamoDbBuilder DynamoDb(string tableName);
        ILambdaBuilder Lambda(string lambdaName);
        IInfrastructureBuilder SnsTopic(string topicName);
        IInfrastructureBuilder Sqs(string queueName);
        IInfrastructureBuilder KafkaTopic(string topicName);
        IInfrastructureBuilder ForRegion(Region region);
    }
}
