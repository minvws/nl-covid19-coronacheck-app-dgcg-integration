// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models;
using System;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Commands
{
    public class HardcodedAppConfigProvider : IAppConfigProvider
    {
        public AppConfigResult Get(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) throw new ArgumentNullException(nameof(type));

            return type switch
            {
                "verifier" => new AppConfigResult
                {
                    IosMinimumVersion = "0",
                    IosAppStoreUrl = "https://www.example.com/myapp",
                    IosMinimumVersionMessage = "Please upgrade",
                    AndroidMinimumVersion = 0,
                    AndroidMinimumVersionMessage = "Please upgrade",
                    PlayStoreUrl = "https://www.example.com/myapp",
                    InformationUrl = "https://www.example.com/myinfo",

                },
                "holder" => new AppConfigResult
                {
                    ProofOfTestValidity = 60 * 3,
                    IosMinimumVersion = "0",
                    IosAppStoreUrl = "https://www.example.com/myapp",
                    IosMinimumVersionMessage = "Please upgrade",
                    AndroidMinimumVersion = 0,
                    AndroidMinimumVersionMessage = "Please upgrade",
                    PlayStoreUrl = "https://www.example.com/myapp",
                    InformationUrl = "https://www.example.com/myinfo"
                },
                _ => throw new InvalidOperationException($"Could not find appconfig of type: {type}")
            };
        }
    }
}