// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2
// using System;

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Extensions;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Signing;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Middleware
{
    public class ResponseSigningMiddleware : IMiddleware
    {
        private const string SignatureHeaderName = "Signature";
        private const string SignatureSmallHeaderName = "SignatureSmall";

        private readonly IContentSigner _signer;

        public ResponseSigningMiddleware(IContentSigner signer)
        {
            _signer = signer ?? throw new ArgumentNullException(nameof(signer));
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            // Store the original response body
            var responseBody = context.Response.Body;

            // Create a new stream for the body for the other middleware to write to
            await using var temporaryResponseBody = new MemoryStream();
            context.Response.Body = temporaryResponseBody;

            // Execute ALL of the other middleware
            await next(context);

            // Calculate signature and add to the response
            SignBody(context.Response);
            SignBodySmall(context.Response);

            // Copy the output of the new stream back to the original one
            await temporaryResponseBody.CopyToAsync(responseBody);
        }

        private void SignBody(HttpResponse response)
        {
            var bodyBytes = response.Body.ReadAllBytes(0);
            var signature = _signer.GetSignature(bodyBytes);
            var signatureB64 = Convert.ToBase64String(signature);
            response.Headers.Add(SignatureHeaderName, new[] { signatureB64 });
        }

        private void SignBodySmall(HttpResponse response)
        {
            var bodyBytes = response.Body.ReadAllBytes(0);
            var signature = _signer.GetSignature(bodyBytes, true);
            var signatureB64 = Convert.ToBase64String(signature);
            response.Headers.Add(SignatureSmallHeaderName, new[] { signatureB64 });
        }
    }
}