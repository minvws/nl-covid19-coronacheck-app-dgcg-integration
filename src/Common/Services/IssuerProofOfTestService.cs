// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.IssuerInterop;
using System;

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

        public string GetProofOfTest(ProofOfTestAttributes proofOfTestAttributes, string nonce, string commitments)
        {
            if (proofOfTestAttributes == null) throw new ArgumentNullException(nameof(proofOfTestAttributes));
            if (string.IsNullOrWhiteSpace(nonce)) throw new ArgumentNullException(nameof(nonce));
            if (string.IsNullOrWhiteSpace(commitments)) throw new ArgumentNullException(nameof(commitments));

            var serializedAttributes = _jsonSerializer.Serialize(proofOfTestAttributes);

            return _issuer
                .IssueProof(_keyStore.GetPublicKey(), _keyStore.GetPrivateKey(), nonce, commitments, serializedAttributes);
        }

        public string GenerateNonce()
        {
            return _issuer.GenerateNonce();
        }
    }
}
