namespace StreetNameRegistry.Api.BackOffice.Infrastructure
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Net.Http.Headers;

    public class CreatedWithETagResult : CreatedResult
    {
        private readonly string _etag;

        public override async Task ExecuteResultAsync(ActionContext context)
        {
            AddEtag(context);
            await base.ExecuteResultAsync(context);
        }

        public override void ExecuteResult(ActionContext context)
        {
            AddEtag(context);
            base.ExecuteResult(context);
        }

        private void AddEtag(ActionContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));


            if (context.HttpContext.Response.Headers.Keys.Contains(HeaderNames.ETag))
                context.HttpContext.Response.Headers[HeaderNames.ETag] = _etag;
            else
                context.HttpContext.Response.Headers.Add(HeaderNames.ETag, _etag);
        }

        public CreatedWithETagResult(Uri location, string etag) : base(location, null)
        {
            _etag = etag;
        }

    }
}
