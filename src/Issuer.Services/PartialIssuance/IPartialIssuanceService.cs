// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Attributes;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.PartialIssuance
{
    public interface IPartialIssuanceService
    {
        ProofOfTestAttributes Apply(ProofOfTestAttributes attributes);
    }
}
