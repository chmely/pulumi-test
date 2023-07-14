using Amazon.DynamoDBv2;
using LetsGetChecked.Infrastructure.Misc;
using Pulumi;
using Pulumi.Aws.DynamoDB.Inputs;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LetsGetChecked.Infrastructure.Builders
{
    public interface IDynamoDbBuilder: IResourceBuilder
    {
        IDynamoDbBuilder WithAttributes(params (string Name, Amazon.DynamoDBv2.ScalarAttributeType Type)[] attributes);
        IDynamoDbBuilder WithRangeKeyColumn(string keyColumnName);
        IDynamoDbBuilder WithHashKeyColumn(string keyColumnName);
        IDynamoDbGlobalIndexBuilder WithGlobalSecondaryIndex(string indexName);
        IDynamoDbBuilder WithStream(StreamViewType streamViewType);
        IInfrastructureBuilder EndDynamoDb();
    }

    public interface IDynamoDbGlobalIndexBuilder
    {
        IDynamoDbGlobalIndexBuilder WithRangeKeyColumn(string keyColumnName);
        IDynamoDbGlobalIndexBuilder WithHashKeyColumn(string keyColumnName);
        IDynamoDbGlobalIndexBuilder WithProjection(ProjectionType projectionType, params string[] columnNames);
        IDynamoDbBuilder EndGlobalSecondaryIndex();
    }

    class DynamoDbGlobalIndexBuilder : ResourceArgBuilder<IDynamoDbBuilder, TableGlobalSecondaryIndexArgs>, IDynamoDbGlobalIndexBuilder
    {
        private readonly DynamoDbBuilder _tableBuilder;

        private readonly TableGlobalSecondaryIndexArgs _index = new ()
        {
            ProjectionType = ProjectionType.ALL.Value
        };

        public DynamoDbGlobalIndexBuilder(DynamoDbBuilder parentBuilder, string indexName) : base(parentBuilder)
        {
            _index.Name = indexName.NotNullOrWhiteSpace(nameof(indexName));
            _tableBuilder = parentBuilder;
        }

        protected override Task<TableGlobalSecondaryIndexArgs> Build()
            => Task.FromResult(_index);

        public IDynamoDbBuilder EndGlobalSecondaryIndex()
            => _parentBuilder;

        public IDynamoDbGlobalIndexBuilder WithHashKeyColumn(string hashKeyColumnName)
        {
            _index.HashKey = hashKeyColumnName.NotNullOrWhiteSpace(nameof(hashKeyColumnName));
            return this;
        }

        public IDynamoDbGlobalIndexBuilder WithProjection(ProjectionType projectionType, params string[] columnNames)
        {
            _index.ProjectionType = projectionType.Value;
            if (projectionType == ProjectionType.INCLUDE)
                _index.NonKeyAttributes = columnNames.ToList();

            return this;
        }

        public IDynamoDbGlobalIndexBuilder WithRangeKeyColumn(string rangeKeyColumnName)
        {
            _index.RangeKey = rangeKeyColumnName.NotNullOrWhiteSpace(nameof(rangeKeyColumnName));
            return this;
        }
    }
}
