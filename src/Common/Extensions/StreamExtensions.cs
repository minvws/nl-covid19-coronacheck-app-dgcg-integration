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
        /// Reads all of the bytes from the given stream.
        /// </summary>
        public static byte[] ReadAllBytes(this Stream stream)
        {
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            return ms.ToArray();
        }

        /// <summary>
        /// Reads all of the bytes from the given stream starting at offset, returns to offset when done
        /// </summary>
        public static byte[] ReadAllBytes(this Stream stream, int startAtOffset)
        {
            stream.Seek(startAtOffset, SeekOrigin.Begin);
            using var ms = new MemoryStream();
            stream.CopyTo(ms);
            stream.Seek(startAtOffset, SeekOrigin.Begin);

            return ms.ToArray();
        }
    }
}