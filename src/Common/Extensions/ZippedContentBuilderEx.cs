// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2
// using System;

using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using System.Threading.Tasks;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Extensions
{
    public static class ZippedContentBuilderEx
    {
        public static async Task<byte[]> BuildStandardAsync(this ZippedContentBuilder thiz, byte[] content, byte[] nlSig)
        {
            var args = new[]
            {
                new ZippedContentBuilderArgs { Value = content, EntryName = "content.bin"},
                new ZippedContentBuilderArgs { Value = nlSig, EntryName = "content.sig"},
            };

            return await thiz.BuildAsync(args);
        }
    }
}
