// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using Microsoft.Extensions.Logging;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Extensions;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Keystores
{
    public class AssemblyKeyStore : IKeyStore
    {
        private const string PrivateKeyPath = @"EmbeddedResources.private_key.xml";
        private const string PublicKeyPath = @"EmbeddedResources.public_key.xml";
        private readonly ILogger<AssemblyKeyStore> _logger;

        public AssemblyKeyStore(ILogger<AssemblyKeyStore> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string GetPrivateKey()
        {
            _logger.LogInformation("Loading private key from the assembly.");

            return typeof(AssemblyKeyStore).Assembly.GetEmbeddedResourceAsString(PrivateKeyPath);
        }

        public string GetPublicKey()
        {
            _logger.LogInformation("Loading private key from the assembly.");

            return typeof(AssemblyKeyStore).Assembly.GetEmbeddedResourceAsString(PublicKeyPath);
        }
    }
}
