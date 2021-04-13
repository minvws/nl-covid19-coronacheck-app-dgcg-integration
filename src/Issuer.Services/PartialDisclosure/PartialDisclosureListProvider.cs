// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.PartialDisclosure
{
    public class PartialDisclosureListProvider : IPartialDisclosureListProvider
    {
        private readonly IPartialDisclosureServiceConfig _config;

        public PartialDisclosureListProvider(IPartialDisclosureServiceConfig config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public IReadOnlyDictionary<string, StopFilter> Execute()
        {
            return PartialDisclosureListLoader.LoadFromFile(_config.Path);
        }
    }
}
