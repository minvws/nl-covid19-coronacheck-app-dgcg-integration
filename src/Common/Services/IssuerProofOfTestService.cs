// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using NL.Rijksoverheid.CoronaTester.BackEnd.IssuerInterop;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services
{
    public class IssuerProofOfTestService : IProofOfTestService
    {
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IKeyStore _keyStore;

        public IssuerProofOfTestService(IJsonSerializer jsonSerializer, IKeyStore keyStore)
        {
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _keyStore = keyStore ?? throw new ArgumentNullException(nameof(keyStore));

        }

        public string GetProofOfTest(string testType, string dateTime, string nonce)
        {
            var commitments = new Commitments
            {
                DateTime = dateTime,
                TestType = testType
            };
            
            return Issuer.IssueProof(_keyStore.GetPrivateKey(), _keyStore.GetPublicKey(), nonce, _jsonSerializer.Serialize(commitments));
        }

        public string GenerateNonce()
        {
            return Issuer.GenerateNonce();
        }
    }
}
