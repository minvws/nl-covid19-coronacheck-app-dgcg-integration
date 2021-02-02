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
        private readonly IIssuerInterop _issuer;

        public IssuerProofOfTestService(IJsonSerializer jsonSerializer, IKeyStore keyStore, IIssuerInterop issuer)
        {
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _keyStore = keyStore ?? throw new ArgumentNullException(nameof(keyStore));
            _issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));

        }

        public string GetProofOfTest(string testType, string dateTime, string nonce, string commitments)
        {
            var attributes = new[] {dateTime, testType};

            return _issuer.IssueProof(_keyStore.GetPublicKey(), _keyStore.GetPrivateKey(), nonce, commitments, _jsonSerializer.Serialize(attributes));
        }

        public string GenerateNonce()
        {
            return _issuer.GenerateNonce();
        }
    }
}
