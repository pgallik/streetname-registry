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
            await Task.Run(() => ExecuteResult(context)).ConfigureAwait(false);
        }

        public override void ExecuteResult(ActionContext context)
        {
            base.ExecuteResult(context);

            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (context.HttpContext.Response.Headers.Keys.Contains(HeaderNames.ETag))
                context.HttpContext.Response.Headers[HeaderNames.ETag] = _etag;
            else
                context.HttpContext.Response.Headers.Add(HeaderNames.ETag, _etag);
        }

        public CreatedWithETagResult(Uri uri, object? value, string etag) : base(uri, value) => _etag = etag;

    }
}
