// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Diagnostics.CodeAnalysis;
using CommandLine;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInteropBenchmark
{
    [SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    internal class Options
    {
        [Option('i', "iterations", Default = 100, HelpText = "Number of iterations to run.", Required = false)]
        public int Iterations { get; set; }

        [Option('n', "nonce", HelpText = "Runs the benchmark for GenerateNonce()", Required = false, Default = true)]
        public bool GenerateNonce { get; set; }

        [Option('p', "issueproof", HelpText = "Runs the benchmark for IssueProof()", Required = false, Default = true)]
        public bool IssueProof { get; set; }
    }
}
