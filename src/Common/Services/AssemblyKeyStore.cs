// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.IO;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services
{
    public class AssemblyKeyStore : IKeyStore
    {
        private const string PrivateKeyPath = @"NL.Rijksoverheid.CoronaTester.BackEnd.Common.Resources.private_key.xml";
        private const string PublicKeyPath = @"NL.Rijksoverheid.CoronaTester.BackEnd.Common.Resources.public_key.xml";

        public string GetPrivateKey()
        {
            return GetFileContent(PrivateKeyPath);
        }

        public string GetPublicKey()
        {
            return GetFileContent(PublicKeyPath);
        }

        private static string GetFileContent(string resourcePath)
        {
            using var stream = typeof(AssemblyKeyStore).Assembly.GetManifestResourceStream(resourcePath);
            if (stream == null)
            {
                throw new Exception("Unable to load key");
            }
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}