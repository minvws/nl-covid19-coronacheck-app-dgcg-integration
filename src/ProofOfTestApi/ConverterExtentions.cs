// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models;
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
                Nonce = nonce,
                Attributes = new Attributes
                {
                    SampleTime = request.Test.Result.SampleDate.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                    TestType = request.Test.Result.TestType,
                    BirthDay = request.Test.Result.Holder.BirthDay,
                    BirthMonth = request.Test.Result.Holder.BirthMonth,
                    FirstNameInitial = request.Test.Result.Holder.FirstNameInitial,
                    LastNameInitial = request.Test.Result.Holder.LastNameInitial
                }
            };
        }

        public static ProofOfTestApi.Models.IssueProofResult ToProofOfTestApiResult(this IssuerApi.Models.IssueProofResult result)
        {
            return new IssueProofResult
            {
                Attributes = result.Attributes,
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

        public static ProofOfTestApi.Models.GenerateNonceResult ToProofOfTestApiResult(this IssuerApi.Models.GenerateNonceResult result, string sessionToken)
        {
            return new GenerateNonceResult
            {
                Nonce = result.Nonce,
                SessionToken = sessionToken
            };
        }
    }
}
