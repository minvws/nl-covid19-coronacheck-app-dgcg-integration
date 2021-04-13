﻿// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInterop
{
    public interface IIssuerInterop
    {
        string GenerateNonce(string publicKeyId);

        string IssueProof(string publicKeyId, string publicKey, string privateKey, string nonce, string commitments, string attributes);

        string IssueStaticDisclosureQr(string publicKeyId, string publicKey, string privateKey, string attributes);
    }
}