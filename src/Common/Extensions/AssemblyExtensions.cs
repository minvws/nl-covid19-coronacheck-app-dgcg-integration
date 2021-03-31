// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2
// using System;

using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Extensions
{
    public static class AssemblyExtensions
    {
        /// <summary>
        ///     Extracts an embedded UTF-8 resource file from the given assembly
        /// </summary>
        /// <param name="assembly">Assembly containing the embedded file</param>
        /// <param name="resourcePath">Path to resource relative to the root (i.e. excluding assembly name)</param>
        /// <returns>File contents in an UTF-8 string</returns>
        public static string GetEmbeddedResourceAsString(this Assembly assembly, string resourcePath)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (string.IsNullOrWhiteSpace(resourcePath)) throw new ArgumentNullException(nameof(resourcePath));

            using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{resourcePath}");
            if (stream == null) throw new InvalidOperationException("Could not find resource.");
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        public static byte[] GetEmbeddedResourceAsBytes(this Assembly assembly, string resourcePath)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (string.IsNullOrWhiteSpace(resourcePath)) throw new ArgumentNullException(nameof(resourcePath));

            var file = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{resourcePath}");
            if (file == null) throw new InvalidOperationException("Could not find resource.");
            var buffer = new byte[file.Length];
            file.Read(buffer, 0, buffer.Length);
            return buffer;
        }
    }
}
