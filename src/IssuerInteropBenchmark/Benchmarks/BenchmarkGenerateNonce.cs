// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Diagnostics;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInteropBenchmark.Benchmarks
{
    internal static class BenchmarkGenerateNonce
    {
        public static void Execute(int iterations, string publicKeyId)
        {
            Console.WriteLine("Benchmarking: GenerateNonce()");

            Console.WriteLine("..Starting the timer..");
            var stopWatch = new Stopwatch();
            stopWatch.Start();

            for (var i = 0; i < iterations; i++)
            {
                var issuer = new IssuerInterop.Issuer();
                issuer.GenerateNonce(publicKeyId);
            }

            stopWatch.Stop();

            Console.WriteLine($"Completed {iterations} iterations in {stopWatch.ElapsedMilliseconds / 1000} seconds.");
        }
    }
}
