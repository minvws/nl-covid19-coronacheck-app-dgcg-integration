// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using CommandLine;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInteropBenchmark.Benchmarks;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInteropBenchmark
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    internal class Program
    {
        private const string PublicKeyId = "testPk";

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options>(args)
                  .WithParsed(Run)
                  .WithNotParsed(HandleParseError);
        }

        private static void Run(Options options)
        {
            if (options.IssueProof) BenchmarkIssueProof.Execute(options.Iterations, PublicKeyId);
            if (options.GenerateNonce) BenchmarkGenerateNonce.Execute(options.Iterations, PublicKeyId);
        }

        private static void HandleParseError(IEnumerable<Error> errs)
        {
            Console.WriteLine("Error parsing input, please check your call and try again.");

            Environment.Exit(1);
        }
    }
}
