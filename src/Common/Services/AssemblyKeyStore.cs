// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Extensions;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services
{
    public class AssemblyKeyStore : IKeyStore
    {
        private const string PrivateKeyPath = @"Resources.private_key.xml";
        private const string PublicKeyPath = @"Resources.public_key.xml";

        public string GetPrivateKey()
        {
            return typeof(AssemblyKeyStore).Assembly.GetEmbeddedResourceAsString(PrivateKeyPath);
        }

        public string GetPublicKey()
        {
            return typeof(AssemblyKeyStore).Assembly.GetEmbeddedResourceAsString(PublicKeyPath);
        }
    }
}