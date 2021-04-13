// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Attributes;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Keystores;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.PartialDisclosure;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInterop;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.ProofOfTest
{
    public class ProofOfTestService : IProofOfTestService
    {
        private readonly IProofOfTestServiceConfig _config;
        private readonly IIssuerInterop _issuer;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly IKeyStore _keyStore;
        private readonly IPartialDisclosureService _partialDisclosureService;

        public ProofOfTestService(IJsonSerializer jsonSerializer, IKeyStore keyStore, IIssuerInterop issuer, IProofOfTestServiceConfig config,
                                  IPartialDisclosureService partialDisclosureService)
        {
            _jsonSerializer = jsonSerializer ?? throw new ArgumentNullException(nameof(jsonSerializer));
            _keyStore = keyStore ?? throw new ArgumentNullException(nameof(keyStore));
            _issuer = issuer ?? throw new ArgumentNullException(nameof(issuer));
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _partialDisclosureService = partialDisclosureService ?? throw new ArgumentNullException(nameof(partialDisclosureService));
        }

        public string GetProofOfTest(ProofOfTestAttributes proofOfTestAttributes, string nonce, string commitments)
        {
            if (proofOfTestAttributes == null) throw new ArgumentNullException(nameof(proofOfTestAttributes));
            if (string.IsNullOrWhiteSpace(nonce)) throw new ArgumentNullException(nameof(nonce));
            if (string.IsNullOrWhiteSpace(commitments)) throw new ArgumentNullException(nameof(commitments));

            var filteredAttributes = _partialDisclosureService.Apply(proofOfTestAttributes);

            var serializedAttributes = _jsonSerializer.Serialize(filteredAttributes);

            return _issuer
               .IssueProof(_config.PublicKeyIdentifier, _keyStore.GetPublicKey(), _keyStore.GetPrivateKey(), nonce, commitments, serializedAttributes);
        }

        public string GenerateNonce()
        {
            return _issuer.GenerateNonce(_config.PublicKeyIdentifier);
        }

        public string GetStaticProofQr(ProofOfTestAttributes proofOfTestAttributes)
        {
            if (proofOfTestAttributes == null) throw new ArgumentNullException(nameof(proofOfTestAttributes));

            var filteredAttributes = _partialDisclosureService.Apply(proofOfTestAttributes);

            var serializedAttributes = _jsonSerializer.Serialize(filteredAttributes);

            return _issuer.IssueStaticDisclosureQr(_config.PublicKeyIdentifier, _keyStore.GetPublicKey(), _keyStore.GetPrivateKey(), serializedAttributes);
        }
    }
}
