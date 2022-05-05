// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2
// using System;

using System;
using System.Reflection;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Extensions
{
    public static class AssemblyExtensions
    {
        public static byte[] GetEmbeddedResourceAsBytes(this Assembly assembly, string resourcePath)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            if (string.IsNullOrWhiteSpace(resourcePath)) throw new ArgumentNullException(nameof(resourcePath));

            var file = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{resourcePath}");
            if (file == null) throw new InvalidOperationException("Could not find resource.");

            var buffer = new byte[file.Length];
            var bytesRead = file.Read(buffer, 0, buffer.Length);
            if (bytesRead <= 0) throw new InvalidOperationException("Embedded resource is empty.");

            return buffer;
        }
    }
}
