using LetsGetChecked.Infrastructure.Misc;
using Pulumi.Aws;
using Pulumi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LetsGetChecked.Infrastructure.Builders
{
    public interface ISqsBuilder : IResourceBuilder
    {
        IInfrastructureBuilder EndSqs();
    }

    internal class SqsBuilder : ResourceBuilder<IInfrastructureBuilder, Pulumi.Kafka.Topic>, ISqsBuilder
    {
        private readonly string _queueName;

        public SqsBuilder(IInfrastructureBuilder parentBuilder, Region region, string queueName) : base(parentBuilder, region)
            => _queueName = queueName.NotNullOrWhiteSpace(nameof(queueName));

        public IInfrastructureBuilder EndSqs()
            => _parentBuilder;

        protected override Task<List<(string Name, CustomResource Resource)>> CreateResources()
        {
            var queue = new Pulumi.Aws.Sqs.Queue(
                _queueName,
                new()
                {
                    MessageRetentionSeconds = (int)TimeSpan.FromDays(7).TotalSeconds
                });

            return ResultAsync((_queueName, queue));
        }
    }
}
