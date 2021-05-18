// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System.Diagnostics.CodeAnalysis;
using CommandLine;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool
{
    // TODO: migrate to verbs :)
    //
    // ReSharper disable ClassNeverInstantiated.Global
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class Options
    {
        [Option('d', "download", Required = false, HelpText = "Download files, ")]
        public bool Download { get; set; }

        [Option('c', "country", Required = false, HelpText = "Only download from the given country (2-digit ISO code)")]
        public string Country { get; set; }

        [Option('t', "type", Required = false, HelpText = "Only download a given type of certification, options are: AUTHENTICATION, UPLOAD, CSCA, DSC")]
        public string Type { get; set; }

        [Option('u', "upload", Required = false, HelpText = "Upload the certificate provided with the -f flag.")]
        public bool Upload { get; set; }

        [Option('r', "revoke", Required = false, HelpText = "Revoke the certificate provided with the -f flag.")]
        public bool Revoke { get; set; }

        [Option('f', "file", Required = false, HelpText = "Path to the file to upload.")]
        public string File { get; set; }

        [Option('o', "output", Required = false, HelpText = "Path to the file where the trustlist output will be written. Overwrites existing files.")]
        public string Output { get; set; }

        [Option('v', "revoke", Required = false, HelpText = "Validate the certificates received from DGCG.")]
        public bool Validate { get; set; }
    }
}
