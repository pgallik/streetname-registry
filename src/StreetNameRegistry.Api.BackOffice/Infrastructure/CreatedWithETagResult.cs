namespace StreetNameRegistry.Api.BackOffice.Infrastructure
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Net.Http.Headers;

    public class CreatedWithETagResult : CreatedResult
    {
        public string ETag { get; }

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
                context.HttpContext.Response.Headers[HeaderNames.ETag] = ETag;
            else
                context.HttpContext.Response.Headers.Add(HeaderNames.ETag, ETag);
        }

        public CreatedWithETagResult(Uri location, string eTag) : base(location, null)
        {
            ETag = eTag;
        }

    }
}
