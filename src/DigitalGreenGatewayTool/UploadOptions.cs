// // Copyright 2022 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System.Diagnostics.CodeAnalysis;
using CommandLine;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool
{
    /// <summary>
    ///     Examples
    ///     dggt upload -f path/to/cert
    ///     dggt upload -f path/to/cert -p
    /// </summary>
    [Verb("upload")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    public class UploadOptions : Options
    {
        [Option('f', "file", Required = false, HelpText = "Path to the file containing the signed (and DER encoded) DSC to upload.")]
        public string File { get; set; }
    }
}
