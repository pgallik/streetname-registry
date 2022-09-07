namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda
{
    using System.Threading;
    using System.Threading.Tasks;
    using Autofac;
    using Be.Vlaanderen.Basisregisters.Aws.Lambda;
    using MediatR;
    using Requests;
    using Sqs.Requests;

    public class MessageHandler : IMessageHandler
    {
        private readonly IContainer _container;

        public MessageHandler(IContainer container)
        {
            _container = container;
        }

        public async Task HandleMessage(object? messageData, MessageMetadata messageMetadata, CancellationToken cancellationToken)
        {
            messageMetadata.Logger?.LogInformation($"Handling message {messageData?.GetType().Name}");

            if (messageData is not SqsRequest sqsRequest)
            {
                messageMetadata.Logger?.LogInformation($"Unable to cast {nameof(messageData)} as {nameof(sqsRequest)}.");
                return;
            }

            await using var lifetimeScope = _container.BeginLifetimeScope();
            var mediator = lifetimeScope.Resolve<IMediator>();

            // TODO: uncomment after initial lambda testing
            //switch (sqsRequest)
            //{
            //    case SqsStreetNameApproveRequest request:
            //        await _mediator.Send(new SqsLambdaStreetNameApproveRequest
            //        {
            //            Request = request.Request,
            //            TicketId = request.TicketId,
            //            MessageGroupId = messageMetadata.MessageGroupId,
            //            IfMatchHeaderValue = request.IfMatchHeaderValue,
            //            Metadata = request.Metadata,
            //            Provenance = request.ProvenanceData.ToProvenance()
            //        }, cancellationToken);
            //        break;

            //    case SqsStreetNameCorrectNamesRequest request:
            //        await _mediator.Send(new SqsLambdaStreetNameCorrectNamesRequest
            //        {
            //            Request = request.Request,
            //            StreetNamePersistentLocalId = request.PersistentLocalId,
            //            TicketId = request.TicketId,
            //            MessageGroupId = messageMetadata.MessageGroupId,
            //            IfMatchHeaderValue = request.IfMatchHeaderValue,
            //            Metadata = request.Metadata,
            //            Provenance = request.ProvenanceData.ToProvenance()
            //        }, cancellationToken);
            //        break;

            //    case SqsStreetNameProposeRequest request:
            //        await _mediator.Send(new SqsLambdaStreetNameProposeRequest
            //        {
            //            Request = request.Request,
            //            TicketId = request.TicketId,
            //            MessageGroupId = messageMetadata.MessageGroupId,
            //            IfMatchHeaderValue = request.IfMatchHeaderValue,
            //            Metadata = request.Metadata,
            //            Provenance = request.ProvenanceData.ToProvenance()
            //        }, cancellationToken);
            //        break;

            //    case SqsStreetNameRejectRequest request:
            //        await _mediator.Send(new SqsLambdaStreetNameRejectRequest
            //        {
            //            Request = request.Request,
            //            TicketId = request.TicketId,
            //            MessageGroupId = messageMetadata.MessageGroupId,
            //            IfMatchHeaderValue = request.IfMatchHeaderValue,
            //            Metadata = request.Metadata,
            //            Provenance = request.ProvenanceData.ToProvenance()
            //        }, cancellationToken);
            //        break;

            //    case SqsStreetNameRetireRequest request:
            //        await _mediator.Send(new SqsLambdaStreetNameRetireRequest
            //        {
            //            Request = request.Request,
            //            TicketId = request.TicketId,
            //            MessageGroupId = messageMetadata.MessageGroupId,
            //            IfMatchHeaderValue = request.IfMatchHeaderValue,
            //            Metadata = request.Metadata,
            //            Provenance = request.ProvenanceData.ToProvenance()
            //        }, cancellationToken);
            //        break;
            //}
        }
    }
}
