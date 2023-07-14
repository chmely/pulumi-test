using LetsGetChecked.Infrastructure;
using Pulumi.Aws;
using Amazon.DynamoDBv2;
using Amazon.Lambda;

Console.WriteLine("nothing");
/*
await
    Infrastructure
        .Builder
            .ForRegion(Region.EUWest1)
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
            .ForRegion(Region.USEast1)
                .Sqs("us-notifications")
                .KafkaTopic("internal-messaging")
            .Create();
*/