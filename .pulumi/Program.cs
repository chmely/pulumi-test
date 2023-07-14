using LetsGetChecked.Infrastructure;
using Pulumi.Aws;
using Amazon.DynamoDBv2;
using Amazon.Lambda;


await
    Infrastructure
        .Builder
            .Sqs("shop-notifications")
            .Create();
/*
await
    Infrastructure
        .Builder
            .DynamoDb("newTable")
                .WithAttributes(
                    ("ID", ScalarAttributeType.N),
                    ("Date", ScalarAttributeType.S),
                    ("Stuff", ScalarAttributeType.S)
                )
                .WithHashKeyColumn("ID")
                .WithRangeKeyColumn("Date")
                .WithStream(StreamViewType.NEW_AND_OLD_IMAGES)
                .WithGlobalSecondaryIndex("index")
                    .WithHashKeyColumn("Stuff")
                    .WithRangeKeyColumn("Date")
                    .WithProjection(ProjectionType.KEYS_ONLY)
                    .EndGlobalSecondaryIndex()
                .EndDynamoDb()
            .Lambda("myLambda")
                .WithHandler("LetsGetChecked.Prescription.KafkaExporter.Lambda::LetsGetChecked.Prescription.KafkaExporter.Lambda.Function::Handler")
                .WithDynamoDbTrigger("newTable", EventSourcePosition.LATEST)
                .EndLambda()
            .Sqs("notifications")
//            .KafkaTopic("internal-messaging")
            .Create();
*/