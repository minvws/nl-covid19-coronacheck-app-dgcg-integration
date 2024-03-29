﻿// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.Extensions.Configuration;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config
{
    public class ApiSigningConfig : AppSettingsReader, IApiSigningConfig
    {
        public ApiSigningConfig(IConfiguration config) : base(config, "ApiSigning")
        {
        }

        public bool WrapAndSignResult => GetConfigValue(nameof(WrapAndSignResult), false);
    }
}
