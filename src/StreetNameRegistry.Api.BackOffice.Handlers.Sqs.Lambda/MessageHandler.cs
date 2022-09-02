namespace StreetNameRegistry.Api.BackOffice.Handlers.Sqs.Lambda
{
    using System.Threading;
    using System.Threading.Tasks;
    using Abstractions.Requests;
    using Be.Vlaanderen.Basisregisters.Aws.Lambda;
    using MediatR;
    using Requests;

    public class MessageHandler : IMessageHandler
    {
        private readonly IMediator _mediator;

        public MessageHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task HandleMessage(object? messageData, MessageMetadata messageMetadata, CancellationToken cancellationToken)
        {
            messageMetadata.Logger?.LogInformation($"Handling message {messageData?.GetType().Name}");

            if (messageData is not SqsLambdaRequest sqsLambdaRequest)
            {
                throw new InvalidOperationException($"Unable to cast {nameof(messageData)} as {nameof(sqsLambdaRequest)}.");
            }

            sqsLambdaRequest.MessageGroupId = messageMetadata.MessageGroupId;

            // TODO: uncomment after initial lambda testing
            //switch (sqsLambdaRequest)
            //{
            //    case SqsLambdaStreetNameApproveRequest request:
            //        await _mediator.Send(request, cancellationToken);
            //        break;

            //    case SqsLambdaStreetNameCorrectNamesRequest request:
            //        await _mediator.Send(request, cancellationToken);
            //        break;

            //    case SqsLambdaStreetNameProposeRequest request:
            //        await _mediator.Send(request, cancellationToken);
            //        break;

            //    case SqsLambdaStreetNameRejectRequest request:
            //        await _mediator.Send(request, cancellationToken);
            //        break;

            //    case SqsLambdaStreetNameRetireRequest request:
            //        await _mediator.Send(request, cancellationToken);
            //        break;
            //}
        }
    }
}
