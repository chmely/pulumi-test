using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using LetsGetChecked.Infrastructure.Misc;
using Pulumi;
using Pulumi.Aws;
using Pulumi.Aws.DynamoDB;
using Pulumi.Aws.DynamoDB.Inputs;
using Pulumi.Aws.Route53.Inputs;

namespace LetsGetChecked.Infrastructure.Builders
{
    class DynamoDbBuilder : ResourceBuilder<IInfrastructureBuilder, Table>, IDynamoDbBuilder
    {
        private readonly string _tableName;
        private readonly TableArgs _tableArgs = new()
        {
            Attributes = new InputList<TableAttributeArgs>(),
            BillingMode = BillingMode.PAY_PER_REQUEST.Value,
            DeletionProtectionEnabled = false
        };

        private List<DynamoDbGlobalIndexBuilder> _globalIndexBuilders = new();

        public DynamoDbBuilder(IInfrastructureBuilder parentBuilder, string tableName, Region region) : base(parentBuilder, region)
            => _tableName = tableName.NotNullOrWhiteSpace(nameof(tableName));

        public IDynamoDbBuilder WithStream(StreamViewType streamViewType)
        {
            _tableArgs.StreamEnabled = true;
            _tableArgs.StreamViewType = streamViewType.Value;

            return this;
        }

        protected override async Task<List<(string Name, CustomResource Resource)>> CreateResources()
        {
            var globalIndexes = await Task.WhenAll(_globalIndexBuilders.Select(gib => gib.Create()).ToArray());

            _tableArgs.GlobalSecondaryIndexes = globalIndexes.ToList();
            _tableArgs.Name = _tableName;

            return Result((_tableName, new Table(_tableName, _tableArgs, ResourceOptions())));
        }

        public IDynamoDbBuilder WithRangeKeyColumn(string rangeKeyColumnName)
        {
            _tableArgs.RangeKey = rangeKeyColumnName;
            return this;
        }

        public IDynamoDbBuilder WithHashKeyColumn(string hashKeyColumnName)
        {
            _tableArgs.HashKey = hashKeyColumnName;
            return this;
        }

        public IDynamoDbBuilder WithAttributes(params (string Name, ScalarAttributeType Type)[] attributes)
        {
            foreach (var (name, type) in attributes)
            {
                _tableArgs.Attributes.Add(new TableAttributeArgs
                {
                    Name = name,
                    Type = type.Value
                });
            }

            return this;
        }

        public IDynamoDbGlobalIndexBuilder WithGlobalSecondaryIndex(string indexName)
        {
            var indexBuilder = new DynamoDbGlobalIndexBuilder(this, indexName);
            _globalIndexBuilders.Add(indexBuilder);
            return indexBuilder;
        }

        public IInfrastructureBuilder EndDynamoDb()
            => _parentBuilder;
    }
}
