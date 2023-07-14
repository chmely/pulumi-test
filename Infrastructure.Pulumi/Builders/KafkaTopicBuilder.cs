using Amazon.DynamoDBv2;
using LetsGetChecked.Infrastructure.Misc;
using Pulumi;
using Pulumi.Aws;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsGetChecked.Infrastructure.Builders
{
    public interface IKafkaTopicBuilder: IResourceBuilder
    {
        IInfrastructureBuilder EndKafkaTopic();
    }

    internal class KafkaTopicBuilder : ResourceBuilder<IInfrastructureBuilder, Pulumi.Kafka.Topic>, IKafkaTopicBuilder
    {
        private readonly string _topicName;

        public KafkaTopicBuilder(IInfrastructureBuilder parentBuilder, Region region, string topicName) : base(parentBuilder, region)
            => _topicName = topicName.NotNullOrWhiteSpace(nameof(topicName));

        public IInfrastructureBuilder EndKafkaTopic()
            => _parentBuilder;

        protected override Task<List<(string Name, CustomResource Resource)>> CreateResources()
        {
            var topic = new Pulumi.Kafka.Topic(
                _topicName,
                new Pulumi.Kafka.TopicArgs
                {
                    Name = _topicName,
                    ReplicationFactor = 1,
                    Partitions = 3
                });

            return ResultAsync((_topicName, topic));
        }
    }
}
