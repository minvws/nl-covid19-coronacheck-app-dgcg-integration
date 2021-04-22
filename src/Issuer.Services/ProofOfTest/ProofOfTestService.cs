// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Attributes;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Keystores;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.PartialIssuance;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInterop;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.ProofOfTest
{
    public class ProofOfTestService : IProofOfTestService
    {
        private readonly IProofOfTestServiceConfig _config;
        private readonly IIssuerInterop _issuer;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IKeyStore _keyStore;
        private readonly IPartialIssuanceService _partialIssuanceService;

        public ProofOfTestService(IJsonSerializer jsonSerializer, IKeyStore keyStore, IIssuerInterop issuer, IPartialIssuanceService partialIssuanceService,
                                  IProofOfTestServiceConfig config)
        {
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _keyStore = keyStore ?? throw new ArgumentNullException(nameof(keyStore));
            _issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
            _partialIssuanceService = partialIssuanceService ?? throw new ArgumentNullException(nameof(partialIssuanceService));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public string GenerateNonce(string keyName)
        {
            if (string.IsNullOrWhiteSpace(keyName)) throw new ArgumentNullException(nameof(keyName));

            return _issuer.GenerateNonce(keyName);
        }

        public (string, ProofOfTestAttributes) GetStaticProofQr(ProofOfTestAttributes proofOfTestAttributes, string nameKeySet)
        {
            if (proofOfTestAttributes == null) throw new ArgumentNullException(nameof(proofOfTestAttributes));
            if (string.IsNullOrWhiteSpace(nameKeySet)) throw new ArgumentNullException(nameof(nameKeySet));

            var keys = _keyStore.GetKeys(nameKeySet);

            var filteredAttributes = _config.EnablePartialIssuanceForStaticProof
                ? _partialIssuanceService.Apply(proofOfTestAttributes)
                : proofOfTestAttributes;

            var serializedAttributes = _jsonSerializer.Serialize(filteredAttributes);

            var proof = _issuer.IssueStaticDisclosureQr(nameKeySet, keys.PublicKey, keys.PrivateKey, serializedAttributes);

            return (proof, filteredAttributes);
        }

        public (string, ProofOfTestAttributes) GetProofOfTest(ProofOfTestAttributes proofOfTestAttributes, string nonce, string commitments, string nameKeySet)
        {
            if (proofOfTestAttributes == null) throw new ArgumentNullException(nameof(proofOfTestAttributes));
            if (string.IsNullOrWhiteSpace(nonce)) throw new ArgumentNullException(nameof(nonce));
            if (string.IsNullOrWhiteSpace(commitments)) throw new ArgumentNullException(nameof(commitments));

            var keys = _keyStore.GetKeys(nameKeySet);

            var filteredAttributes = _config.EnablePartialIssuanceForDynamicProof
                ? _partialIssuanceService.Apply(proofOfTestAttributes)
                : proofOfTestAttributes;

            var serializedAttributes = _jsonSerializer.Serialize(filteredAttributes);

            var proof = _issuer.IssueProof(nameKeySet, keys.PublicKey, keys.PrivateKey, nonce, commitments, serializedAttributes);

            return (proof, filteredAttributes);
        }
    }
}
