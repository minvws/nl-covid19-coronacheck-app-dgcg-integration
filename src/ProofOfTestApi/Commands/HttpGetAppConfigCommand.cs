using Microsoft.AspNetCore.Http;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models;
using System;
using System.Text;
using System.Threading.Tasks;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Commands
{
    public class HttpGetAppConfigCommand
    {
        private readonly ZippedSignedContentFormatter _formatter;
        private readonly IJsonSerializer _serializer;

        public HttpGetAppConfigCommand(ZippedSignedContentFormatter formatter, IJsonSerializer serializer)
        {
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public async Task ExecuteAsync(HttpContext context)
        {
            var testJson = _serializer.Serialize(new AppConfigResult
            {
                MinimumVersionIos = "0",
                MinimumVersionAndroid = "0",
                MinimumVersionMessage = "Please upgrade",
                AppStoreUrl = "https://www.example.com/myapp",
                InformationUrl = "https://www.example.com/myinfo"
            });
            
            var testJsonBytes = Encoding.UTF8.GetBytes(testJson);

            var content = await _formatter.SignedContentPacketAsync(testJsonBytes);

            await context.Response.Body.WriteAsync(content);
        }
    }
}