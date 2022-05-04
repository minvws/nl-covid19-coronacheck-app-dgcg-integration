// // Copyright 2022 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System.Diagnostics.CodeAnalysis;
using CommandLine;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool
{
    /// <summary>
    ///     Examples
    ///     dggt revoke -f path/to/cert
    ///     dggt revoke -f path/to/cert -p
    /// </summary>
    [Verb("revoke")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class RevokeOptions : Options
    {
        [Option('f', "file", Required = false, HelpText = "Path to the file containing the signed(and DER encoded) DSC to revoke.")]
        public string File { get; init; }
    }
}
