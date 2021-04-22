// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.IO;
using System.Text;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.PartialIssuance;
using Xunit;

namespace Issuer.Services.Tests.PartialIssuance
{
    public class PartialIssuanceListLoaderTests
    {
        [Fact]
        public void TestLoadFromString()
        {
            // Assemble
            var sb = new StringBuilder();
            sb.AppendLine("AA,VFMD");
            sb.AppendLine("AB,V");
            sb.AppendLine("AC,FMD");

            // Act
            var result = WhitelistLoader.LoadFromString(sb.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains("AA", result);
            Assert.True(result["AA"].AllowFirstInitial);
            Assert.True(result["AA"].AllowLastInitial);
            Assert.True(result["AA"].AllowMonth);
            Assert.True(result["AA"].AllowDay);
        }

        [Fact]
        public void TestLoadFromFile()
        {
            var fileName = $"{Path.GetTempPath()}{Guid.NewGuid()}.csv";

            try
            {
                // Assemble
                var data = new[] {"AA,VFMD", "AB,V", "AC,FMD"};
                File.WriteAllLines(fileName, data);

                // Act
                var result = WhitelistLoader.LoadFromFile(fileName);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(3, result.Count);
                Assert.Contains("AA", result);
                Assert.True(result["AA"].AllowFirstInitial);
                Assert.True(result["AA"].AllowLastInitial);
                Assert.True(result["AA"].AllowMonth);
                Assert.True(result["AA"].AllowDay);
            }
            finally
            {
                File.Delete(fileName);
            }
        }

        [Theory]
        [InlineData("V")]
        [InlineData("F")]
        [InlineData("M")]
        [InlineData("D")]
        [InlineData("VF")]
        [InlineData("VM")]
        [InlineData("VD")]
        [InlineData("FM")]
        [InlineData("FD")]
        [InlineData("MD")]
        [InlineData("VFM")]
        [InlineData("VFD")]
        [InlineData("VMD")]
        [InlineData("FMD")]
        [InlineData("VFMD")]
        public void TestLoadFromStringCombinatorial(string attributesToIssue)
        {
            // Assemble
            var sb = new StringBuilder();
            sb.AppendLine($"AA,{attributesToIssue}");

            // Act
            var result = WhitelistLoader.LoadFromString(sb.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Contains("AA", result);
            if (attributesToIssue.Contains("V")) Assert.True(result["AA"].AllowFirstInitial);
            if (attributesToIssue.Contains("F")) Assert.True(result["AA"].AllowLastInitial);
            if (attributesToIssue.Contains("M")) Assert.True(result["AA"].AllowMonth);
            if (attributesToIssue.Contains("D")) Assert.True(result["AA"].AllowDay);
        }
    }
}
