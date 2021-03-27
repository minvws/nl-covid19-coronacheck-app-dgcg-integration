// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Models;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerApi.Models;
using GenerateNonceResult = NL.Rijksoverheid.CoronaCheck.BackEnd.ProofOfTestApi.Models.GenerateNonceResult;
using IssueProofResult = NL.Rijksoverheid.CoronaCheck.BackEnd.ProofOfTestApi.Models.IssueProofResult;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.ProofOfTestApi
{
    public static class ConverterExtensions
    {
        public static IssueProofRequest ToIssuerApiRequest(this Models.IssueProofRequest request, string nonce)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(nonce)) throw new ArgumentException(nameof(request));
            if (request.Test == null) throw new InvalidOperationException("Test result is null");

            return new IssueProofRequest
            {
                Commitments = request.Commitments,
                Nonce = nonce,
                Attributes = new Attributes
                {
                    SampleTime = request.Test.Result!.SampleDate,
                    TestType = request.Test.Result.TestType,
                    BirthDay = request.Test.Result.Holder!.BirthDay,
                    BirthMonth = request.Test.Result.Holder.BirthMonth,
                    FirstNameInitial = request.Test.Result.Holder.FirstNameInitial,
                    LastNameInitial = request.Test.Result.Holder.LastNameInitial
                }
            };
        }

        public static IssueProofResult ToProofOfTestApiResult(this IssuerApi.Models.IssueProofResult result)
        {
            if (result == null) throw new ArgumentNullException(nameof(result));
            if (result.Ism?.Proof?.C == null) throw new InvalidOperationException("ISM proof is null");

            return new IssueProofResult
            {
                Attributes = result.Attributes,
                Ism = new IssueSignatureMessage
                {
                    Proof = new Proof
                    {
                        C = result.Ism!.Proof!.C,
                        ErrorResponse = result.Ism.Proof.ErrorResponse
                    },
                    Signature = result.Ism.Signature
                }
            };
        }

        public static GenerateNonceResult ToProofOfTestApiResult(this IssuerApi.Models.GenerateNonceResult result, string sessionToken)
        {
            return new GenerateNonceResult
            {
                Nonce = result.Nonce,
                SessionToken = sessionToken
            };
        }
    }
}
