// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Threading.Tasks;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Models;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Client
{
    public interface IIssuerApiClient
    {
        Task<IssueProofResult> IssueProof(IssueProofRequest request);
        Task<GenerateNonceResult> GenerateNonce();
        Task<IssueStaticProofResult> IssueStaticProof(IssueStaticProofRequest request);
    }
}
