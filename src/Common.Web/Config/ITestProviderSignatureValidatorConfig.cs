// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Collections.Generic;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Config
{
    public interface ITestProviderSignatureValidatorConfig
    {
        /// <summary>
        /// Key: providerId, Value: path to their signing certificate on the filesystem
        /// </summary>
        Dictionary<string, string> ProviderCertificates { get; }
    }
}