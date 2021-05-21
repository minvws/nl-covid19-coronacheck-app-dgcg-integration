// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Collections.Generic;
using System.Text.Json;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Formatters
{
    /// <summary>
    ///     Outputs JSON in the DGCG format
    /// </summary>
    public class DgcgJsonFormatter : ITrustListFormatter
    {
        public string Format(IEnumerable<TrustListItem> trustList)
        {
            return JsonSerializer.Serialize(trustList);
        }
    }
}
