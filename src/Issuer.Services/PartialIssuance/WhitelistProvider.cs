// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.PartialIssuance
{
    public class WhitelistProvider : IWhitelistProvider
    {
        private readonly IPartialIssuanceServiceConfig _config;

        public WhitelistProvider(IPartialIssuanceServiceConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public IReadOnlyDictionary<string, WhitelistItem> Execute()
        {
            return WhitelistLoader.LoadFromFile(_config.WhitelistPath);
        }
    }
}
