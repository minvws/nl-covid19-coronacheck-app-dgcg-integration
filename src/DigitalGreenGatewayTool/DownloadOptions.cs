﻿// // Copyright 2022 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System;
using System.Diagnostics.CodeAnalysis;
using CommandLine;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool
{
    [Verb("download")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    public class DownloadOptions : Options
    {
        [Option('c', "country", Required = false, HelpText = "Only download from the given country (2-digit ISO code)")]
        public string Country { get; set; }

        [Option('t', "type", Required = false, HelpText = "Only download a given type of certification, options are: AUTHENTICATION, UPLOAD, CSCA, DSC")]
        public string Type { get; set; }

        [Option('f', "file", Required = false, HelpText = "Path to the file to upload.")]
        public string File { get; set; }

        [Option('o', "output", Required = false, HelpText = "Path to the file where the trust-list output will be written. Overwrites existing files.")]
        public string Output { get; set; }

        [Option("unformatted", Required = false, HelpText = "Output the raw unformatted TrustList JSON instead of the Dutch packaged format.")]
        public bool Unformatted { get; set; }

        [Option('v', "validate", Required = false, HelpText = "Validate the certificates received from DGCG.")]
        public bool Validate { get; set; }

        [Option("third-party-keys-file", Required = false,
                HelpText =
                    "Path to the file to containing extra external keys which will be injected into the trust-list, formatted as a DGCG trust list json. The filename must be the ISO-3166-2 country code of the third country")]
        public string ThirdPartyKeysFile { get; set; }

        public void ValidateSelectedOptions()
        {
            // Type
            if (!string.IsNullOrWhiteSpace(Type))
            {
                switch (Type)
                {
                    case "AUTHENTICATION":
                        break;
                    case "UPLOAD":
                        break;
                    case "CSCA":
                        break;
                    case "DSC":
                        break;
                    default:
                        Console.WriteLine($"ERROR: Invalid type `{Type}`, acceptable values are: `AUTHENTICATION`, `UPLOAD`, `CSCA`, `DSC`.");
                        Environment.Exit(1);
                        break;
                }

                if (Validate) Console.WriteLine("WARNING: you are downloading trust list items of a specific type, this may cause the validation to fail.");
            }

            // Country
            if (string.IsNullOrWhiteSpace(Country)) return;
            if (Country.Length == 2) return;

            Console.WriteLine($"ERROR: Invalid country `{Country}`, please use a two-digit ISO code.");
            Environment.Exit(1);
        }
    }
}