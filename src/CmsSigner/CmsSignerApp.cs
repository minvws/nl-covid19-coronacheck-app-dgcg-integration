// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System;
using System.Text;
using CmsSigner.Model;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Signing;

namespace CmsSigner
{
    internal class CmsSignerApp
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IContentSigner _signer;
        private readonly ICmsValidator _validator;

        public CmsSignerApp(IJsonSerializer jsonSerializer, ICmsValidator validator, IContentSigner signer)
        {
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _signer = signer ?? throw new ArgumentNullException(nameof(signer));
        }

        public void Run(byte[] inputFile, bool validate)
        {
            if (validate)
            {
                var json = Encoding.UTF8.GetString(inputFile);
                var signedDataResponse = _jsonSerializer.Deserialize<SignedDataResponse>(json);
                var signatureBytes = Convert.FromBase64String(signedDataResponse.Signature!);
                var payloadBytes = Convert.FromBase64String(signedDataResponse.Payload!);

                var valid = _validator.Validate(payloadBytes, signatureBytes);

                Console.WriteLine(valid ? "true" : "false");

                Environment.Exit(0);
            }

            var result = new SignedDataResponse
            {
                Payload = Convert.ToBase64String(inputFile),
                Signature = Convert.ToBase64String(_signer.GetSignature(inputFile))
            };

            var resultJson = _jsonSerializer.Serialize(result);

            Console.Write(resultJson);
        }
    }
}
