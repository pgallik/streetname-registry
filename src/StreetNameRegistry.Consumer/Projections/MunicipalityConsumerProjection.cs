namespace StreetNameRegistry.Consumer.Projections
{
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.Connector;
    using Be.Vlaanderen.Basisregisters.ProjectionHandling.SqlStreamStore;
    using Municipality;
    using StreetName.Events;

    public class MunicipalityConsumerProjection : ConnectedProjection<ConsumerContext>
    {
        public MunicipalityConsumerProjection()
        {
            When<Envelope<MunicipalityWasImported>>(async (context, message, ct) =>
            {
                await context.MunicipalityConsumerItems.AddAsync(new MunicipalityConsumerItem
                {
                    MunicipalityId = message.Message.MunicipalityId,
                    NisCode = message.Message.NisCode
                }, ct);
            });

            When<Envelope<MunicipalityNisCodeWasChanged>>(async (context, message, ct) =>
            {
                var item = await context.FindAsync<MunicipalityConsumerItem>(new object[] { message.Message.MunicipalityId }, ct);
                item.NisCode = message.Message.NisCode;
            });
        }
    }
}
