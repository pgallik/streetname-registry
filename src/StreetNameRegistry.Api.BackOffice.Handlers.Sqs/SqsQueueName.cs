namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs
{
    internal static class SqsQueueName
    {
        public const string Value = $"{nameof(StreetNameRegistry)}.{nameof(Api)}.{nameof(BackOffice)}";
    }
}
