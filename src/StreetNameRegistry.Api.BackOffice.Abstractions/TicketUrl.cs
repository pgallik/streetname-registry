namespace StreetNameRegistry.Api.BackOffice.Abstractions
{
    using System;
    using TicketingService.Abstractions;

    //TODO: move to Ticketing Lib?
    public class TicketingUrl : ITicketingUrl
    {
        private readonly string _baseUrl;
        public string Scheme { get; }
        public string Host { get; }
        public string PathBase { get; }

        public TicketingUrl(string baseUrl)
        {
            _baseUrl = baseUrl;
            var uri = new Uri(_baseUrl);
            Scheme = uri.Scheme;
            Host = uri.Host;
            PathBase = uri.PathAndQuery;
        }

        public string For(Guid ticketId)
        {
            if (_baseUrl.EndsWith("/"))
            {
                return $"{_baseUrl}{ticketId:D}";
            }

            return $"{_baseUrl}/{ticketId:D}";
        }
    }
}
