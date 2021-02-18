// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using CmsSigner.Certificates;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Signing;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;

namespace CmsSigner
{
    internal class Program
    {
        private const string Example = "CmsSigner file_to_sign.xyz [path-to-x509-CMS-certificate] [password] [path-to-x509-CMS-chain]";

        private static void Main(string[] args)
        {
            if (args == null
                || args.Length != 4
                || string.IsNullOrWhiteSpace(args[0])
                || string.IsNullOrWhiteSpace(args[1])
                || string.IsNullOrWhiteSpace(args[2])
                || string.IsNullOrWhiteSpace(args[3]))
            {
                Console.WriteLine("Something bad happened, this is how you call me: ");
                Console.WriteLine();
                Console.WriteLine(Example);

                Environment.Exit(1);
            }

            try
            {
                Run(args);
            }
            catch (Exception e)
            {
                Console.WriteLine("Something bad happened! See exception message for details:");
                Console.WriteLine($"{e.Message}");

                Environment.Exit(2);
            }
        }

        private static void Run(IReadOnlyList<string> args)
        {
            var inputFile = Load(args[0]);
            var certFile = Load(args[1]);
            var password = args[2];
            var chainFile = Load(args[3]);

            var certProvider = new CertProvider(CertificateHelpers.Load(certFile, password));
            var chainProvider = new ChainProvider(CertificateHelpers.LoadAll(chainFile));
            var signer = new CmsSignerEnhanced(certProvider, chainProvider, new StandardUtcDateTimeProvider());
            var serializer = new StandardJsonSerializer();

            var result = new SignedDataResponse<object>
            {
                Payload = Convert.ToBase64String(inputFile),
                Signature = Convert.ToBase64String(signer.GetSignature(inputFile))
            };


            var resultJson = serializer.Serialize(result);

            Console.Write(resultJson);
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
    }
}
