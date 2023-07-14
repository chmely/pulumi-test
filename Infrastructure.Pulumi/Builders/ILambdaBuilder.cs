namespace LetsGetChecked.Infrastructure.Builders
{
    public interface ILambdaBuilder : IResourceBuilder
    {
        ILambdaBuilder WithDynamoDbTrigger(string tableName, Amazon.Lambda.EventSourcePosition sourcePosition);
        ILambdaBuilder WithHandler(string handler);
        IInfrastructureBuilder EndLambda();
    }
}
