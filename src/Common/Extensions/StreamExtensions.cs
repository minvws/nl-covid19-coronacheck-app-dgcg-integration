// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2
// using System;

using System.IO;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Extensions
{
    public static class StreamExtensions
    {
        /// <summary>
        /// Reads all of the bytes from the given stream from the first position without changing the current position in the stream.
        /// </summary>
        public static byte[] ReadAllBytes(this Stream stream)
        {
            var startPosition = stream.Position;
            stream.Seek(0, SeekOrigin.Begin);
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            stream.Seek(startPosition, SeekOrigin.Begin);

            return ms.ToArray();
        }
    }
}