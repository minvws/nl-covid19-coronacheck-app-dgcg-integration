using Microsoft.AspNetCore.Http;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.ExposureNotification.BackEnd.Crypto.Signing;
using System;
using System.Text;
using System.Threading.Tasks;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Commands
{
    public class HttpGetAppConfigCommand
    {
        private readonly ZippedSignedContentFormatter _formatter;

        public HttpGetAppConfigCommand(ZippedSignedContentFormatter formatter)
        {
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        }

        public async Task ExecuteAsync(HttpContext context)
        {
            const string testJson = "{ message: \"Hello World\" }";

            var testJsonBytes = Encoding.UTF8.GetBytes(testJson);

            var content = await _formatter.SignedContentPacketAsync(testJsonBytes);

            //_CacheControlHeaderProcessor.Execute(httpContext, e);

            await context.Response.Body.WriteAsync(content);
        }
    }
}