// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Formatters
{
    /// <summary>
    ///     Outputs JSON in the DGCG format
    /// </summary>
    public class DgcgJsonFormatter : ITrustListFormatter
    {
        private readonly IJsonSerializer _jsonSerializer;

        public DgcgJsonFormatter(IJsonSerializer serializer)
        {
            _jsonSerializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public string Format(IEnumerable<TrustListItem> trustList, string thirdPartyKeyFile)
        {
            // Third-party keys aren't supported here to are just ignored
            return _jsonSerializer.Serialize(trustList);
        }
    }
}
