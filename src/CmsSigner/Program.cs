// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using CmsSigner.Certificates;
using CmsSigner.Model;
using CommandLine;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Signing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using static System.Convert;

namespace CmsSigner
{
    internal class Program
    {
        public class Options
        {
            [Option('i', "inputFile", Required = true, HelpText = "File to sign (Validate=False) | File containing json wrapper with payload/signature to validate (Validate=True")]
            public string InputFile { get; set; }
            
            [Option('s', "signingCertFile", Required = true, HelpText = "Certificate (pfx, including private key) used to sign the input file")]
            public string SigningCertificateFile { get; set; }

            [Option('p', "password", Required = true, HelpText = "Password for the private key associated with the signing certificate")]
            public string Password { get; set; }
            
            [Option('c', "certChainFile", Required = true, HelpText = "Certificate chain (p7b) for the signing certificate")]
            public string CertificateChainFile { get; set; }

            [Option('v', "validate", Required = false, HelpText = "Instead of signing inputFile, instead check the signature in signatureFile matches that from inputFile.")]
            public bool Validate { get; set; }
        }


        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                .WithParsed(Run)
                .WithNotParsed(HandleParseError);
        }
        
        private static void Run(Options options)
        {
            var inputFile = Load(options.InputFile);
            var certFile = Load(options.SigningCertificateFile);
            var chainFile = Load(options.CertificateChainFile);

            var certProvider = new CertProvider(CertificateHelpers.Load(certFile, options.Password));
            var chainProvider = new ChainProvider(CertificateHelpers.LoadAll(chainFile));
            var signer = new CmsSignerEnhanced(certProvider, chainProvider, new StandardUtcDateTimeProvider());
            var serializer = new StandardJsonSerializer();

            if (options.Validate)
            {
                var validator = new CmsValidatorEnhanced(certProvider, chainProvider);
                var json = System.Text.Encoding.UTF8.GetString(inputFile);
                var signedDataResponse = serializer.Deserialize<SignedDataResponse>(json);
                var signatureBytes = FromBase64String(signedDataResponse.Signature);
                var payloadBytes = FromBase64String(signedDataResponse.Payload);

                var valid = validator.Validate(payloadBytes, signatureBytes);

                Console.WriteLine(valid ? "true" : "false");

                Environment.Exit(0);
            }

            var result = new SignedDataResponse
            {
                Payload = ToBase64String(inputFile),
                Signature = ToBase64String(signer.GetSignature(inputFile))
            };

            var resultJson = serializer.Serialize(result);
            
            Console.Write(resultJson);
        }

        public static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("Error parsing input, please check your call and try again.");

            Environment.Exit(1);
        }

        private static byte[] Load(string path)
        {
            try
            {
                return File.ReadAllBytes(path);
            }
            catch
            {
                // ignored
            }
            
            Console.WriteLine($"ERROR: unable to open file: {path}");

            Environment.Exit(1);

            return null;
        }
        
        //private static bool Validate(ICmsValidator signer, byte[] file)
        //{
        //    var serializer = new StandardJsonSerializer();
        //    var json = System.Text.Encoding.UTF8.GetString(file);
        //    var signedDataResponse = serializer.Deserialize<SignedDataResponse>(json);
        //    var signatureBytes = FromBase64String(signedDataResponse.Signature);
        //    var payloadBytes = FromBase64String(signedDataResponse.Payload);

        //    return signer.Validate(payloadBytes, signatureBytes);

        //    //try
        //    //{
        //    //    var contentInfo = new ContentInfo(payloadBytes);
        //    //    var cms = new SignedCms(contentInfo, true);
        //    //    cms.Decode(signatureBytes);
        //    //    cms.CheckSignature(true);
        //    //}
        //    //catch (CryptographicException)
        //    //{
        //    //    return false;
        //    //}
        //    //return true;
        //}
    }
}
