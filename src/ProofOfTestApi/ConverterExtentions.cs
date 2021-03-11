// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Models;
using IssueProofRequest = NL.Rijksoverheid.CoronaTester.BackEnd.IssuerApi.Models.IssueProofRequest;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi
{
    public static class ConverterExtensions
    {
        public static IssuerApi.Models.IssueProofRequest ToIssuerApiRequest(this ProofOfTestApi.Models.IssueProofRequest request, string nonce)
        {
            return new IssueProofRequest
            {
                Commitments = request.Commitments,
                SampleTime = request.SampleTime,
                TestType = request.TestType,
                Nonce = nonce
            };
        }

        public static ProofOfTestApi.Models.IssueProofResult ToProofOfTestApiResult(this IssuerApi.Models.IssueProofResult result)
        {
            return new IssueProofResult
            {
                Attributes = new Attributes
                {
                    SampleTime = result.Attributes.SampleTime,
                    TestType = result.Attributes.TestType
                },
                Ism = new IssueSignatureMessage
                {
                    Proof = new Proof
                    {
                        C = result.Ism.Proof.C,
                        ErrorResponse = result.Ism.Proof.ErrorResponse
                    },
                    Signature = result.Ism.Signature
                }
            };
        }

        public static ProofOfTestApi.Models.GenerateNonceResult ToProofOfTestApiResult(this IssuerApi.Models.GenerateNonceResult result)
        {
            return new GenerateNonceResult
            {
                Nonce = result.Nonce
            };
        }
    }
}
