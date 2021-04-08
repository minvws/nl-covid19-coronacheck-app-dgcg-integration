// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Moq;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.HolderInterop;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInterop;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInteropBenchmark.Benchmarks
{
    internal static class BenchmarkIssueProof
    {
        public static void Execute(int iterations, string publicKeyId)
        {
            Console.WriteLine("Benchmarking: IssueProof()");

            Console.WriteLine("..Setting up the Holder..");
            var holder = new Holder();
            holder.LoadIssuerPks(Keys.AnnotatedKeys());
            var holderSecretKey = holder.GenerateHolderSecretKey();

            Console.WriteLine("..Setting up the Issuer..");
            var issuer = new Issuer();
            var keystore = new AssemblyKeyStore(new Mock<ILogger<AssemblyKeyStore>>().Object);
            var issuerPkXml = keystore.GetPublicKey();
            var issuerSkXml = keystore.GetPrivateKey();
            var attributeGenerator = new RandomAttributesGenerator();

            Console.WriteLine($"..Generating {iterations} test cases.. this may take some time..");
            var cases = new List<(string, string, string)>(iterations);
            for (var i = 0; i < iterations; i++)
            {
                if (i % 10 == 0) Console.WriteLine($"....Generating case {i}..");

                var nonce = issuer.GenerateNonce(publicKeyId);
                var commitments = holder.CreateCommitmentMessage(holderSecretKey, nonce);
                var attributes = JsonSerializer.Serialize(attributeGenerator.Generate());

                cases.Add((nonce, commitments, attributes));
            }

            Console.WriteLine($"..Generated {iterations} test cases!");

            Console.WriteLine("..Starting the timer..");
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            Console.WriteLine("..Issuing {iterations} proof");
            foreach (var (nonce, commitments, attributes) in cases) issuer.IssueProof(publicKeyId, issuerPkXml, issuerSkXml, nonce, commitments, attributes);

            stopWatch.Stop();
            Console.WriteLine($"Completed {iterations} iterations in {stopWatch.ElapsedMilliseconds / 1000} seconds.");
        }
    }
}
