// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Collections.Generic;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Formatters;

public interface ITrustListFormatter
{
    string Format(IEnumerable<TrustListItem> trustList, string thirdPartyKeyFile);
}
