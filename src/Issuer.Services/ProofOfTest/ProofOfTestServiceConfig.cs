// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.Extensions.Configuration;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.ProofOfTest
{
    public class ProofOfTestServiceConfig : AppSettingsReader, IProofOfTestServiceConfig
    {
        public ProofOfTestServiceConfig(IConfiguration config) : base(config, "Issuer")
        {
        }

        public string PublicKeyIdentifier => GetConfigValue<string>(nameof(PublicKeyIdentifier));
    }
}
