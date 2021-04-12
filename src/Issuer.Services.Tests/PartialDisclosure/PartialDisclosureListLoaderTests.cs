// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.IO;
using System.Text;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.PartialDisclosure;
using Xunit;

namespace Issuer.Services.Tests.PartialDisclosure
{
    public class PartialDisclosureListLoaderTests
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
            var result = PartialDisclosureListLoader.LoadFromString(sb.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count);
            Assert.Contains("AA", result);
            Assert.True(result["AA"].DiscloseFirstInitial);
            Assert.True(result["AA"].DiscloseLastInitial);
            Assert.True(result["AA"].DiscloseMonth);
            Assert.True(result["AA"].DiscloseDay);
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
                var result = PartialDisclosureListLoader.LoadFromFile(fileName);

                // Assert
                Assert.NotNull(result);
                Assert.Equal(3, result.Count);
                Assert.Contains("AA", result);
                Assert.True(result["AA"].DiscloseFirstInitial);
                Assert.True(result["AA"].DiscloseLastInitial);
                Assert.True(result["AA"].DiscloseMonth);
                Assert.True(result["AA"].DiscloseDay);
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
        public void TestLoadFromStringCombinatorial(string disclosures)
        {
            // Assemble
            var sb = new StringBuilder();
            sb.AppendLine($"AA,{disclosures}");

            // Act
            var result = PartialDisclosureListLoader.LoadFromString(sb.ToString());

            // Assert
            Assert.NotNull(result);
            Assert.Contains("AA", result);
            if (disclosures.Contains("V")) Assert.True(result["AA"].DiscloseFirstInitial);
            if (disclosures.Contains("F")) Assert.True(result["AA"].DiscloseLastInitial);
            if (disclosures.Contains("M")) Assert.True(result["AA"].DiscloseMonth);
            if (disclosures.Contains("D")) Assert.True(result["AA"].DiscloseDay);
        }
    }
}
