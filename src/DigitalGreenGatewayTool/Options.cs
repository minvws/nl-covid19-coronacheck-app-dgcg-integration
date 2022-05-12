// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Diagnostics.CodeAnalysis;
using CommandLine;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public abstract class Options
{
    [Option('p', "pause", Required = false, HelpText = "Pause after execution until a key is pressed.")]
    public bool Pause { get; set; }
}
