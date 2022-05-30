// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System.Diagnostics.CodeAnalysis;
using CommandLine;

namespace CmsSigner;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
internal abstract class Options
{
    [Option('i', "inputFile", Required = true,
            HelpText = "File to sign (Validate=False) | File containing json wrapper with payload/signature to validate (Validate=True")]
    public string? InputFile { get; set; }

    [Option('s', "signingCertFile", Required = true, HelpText = "Certificate (pfx, including private key) used to sign the input file")]
    public string? SigningCertificateFile { get; set; }

    [Option('p', "password", Required = true, HelpText = "Password for the private key associated with the signing certificate")]
    public string? Password { get; set; }

    [Option('c', "certChainFile", Required = true, HelpText = "Certificate chain (p7b) for the signing certificate")]
    public string? CertificateChainFile { get; set; }

    [Option('v', "validate", Required = false,
            HelpText = "Instead of signing inputFile, instead check the signature in signatureFile matches that from inputFile.")]
    public bool Validate { get; set; }
}
