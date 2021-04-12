// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using Moq;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Attributes;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.PartialDisclosure;
using Xunit;

namespace Issuer.Services.Tests.PartialDisclosure
{
    public class PartialDisclosureServiceTest
    {
        [Fact]
        public void Apply_returns_new_attributes_on_match()
        {
            // Assemble
            var stopList = new Dictionary<string, StopFilter> {{"AB", new StopFilter(true, true, true, true)}};
            var provider = new Mock<IPartialDisclosureListProvider>();
            provider.Setup(x => x.Execute()).Returns(stopList);
            var service = new PartialDisclosureService(provider.Object);

            // Act
            var attributes = new ProofOfTestAttributes(DateTime.UtcNow, "ZXY", "A", "B", "2", "1", true, true);
            var result = service.Apply(attributes);

            // Assert
            Assert.NotNull(result);
            Assert.NotSame(attributes, result);
        }

        [Fact]
        public void Apply_matches_only_one_item()
        {
            // Assemble
            var stopList = new Dictionary<string, StopFilter>
            {
                {"AA", new StopFilter(true, true, true, true)},
                {"AB", new StopFilter(false, false, false, false)}
            };
            var provider = new Mock<IPartialDisclosureListProvider>();
            provider.Setup(x => x.Execute()).Returns(stopList);
            var service = new PartialDisclosureService(provider.Object);

            // Act
            var attributes = new ProofOfTestAttributes(DateTime.UtcNow, "ZXY", "A", "B", "2", "1", true, true);
            var result = service.Apply(attributes);

            // Assert
            Assert.NotNull(result);
            Assert.NotSame(attributes, result);
            Assert.Equal(string.Empty, result.FirstNameInitial);
            Assert.Equal(string.Empty, result.LastNameInitial);
            Assert.Equal(string.Empty, result.BirthMonth);
            Assert.Equal(string.Empty, result.BirthDay);
        }

        [Theory]
        [InlineData(false, false, false, false)]
        [InlineData(false, false, false, true)]
        [InlineData(false, false, true, false)]
        [InlineData(false, false, true, true)]
        [InlineData(false, true, false, false)]
        [InlineData(false, true, false, true)]
        [InlineData(false, true, true, false)]
        [InlineData(false, true, true, true)]
        [InlineData(true, false, false, false)]
        [InlineData(true, false, false, true)]
        [InlineData(true, false, true, false)]
        [InlineData(true, false, true, true)]
        [InlineData(true, true, false, false)]
        [InlineData(true, true, false, true)]
        [InlineData(true, true, true, false)]
        [InlineData(true, true, true, true)]
        public void Apply_executes_expected_partial_disclosures(bool discloseFirstInitial, bool discloseLastInitial, bool discloseDay, bool discloseMonth)
        {
            // Assemble
            var stopList = new Dictionary<string, StopFilter> {{"AB", new StopFilter(discloseFirstInitial, discloseLastInitial, discloseDay, discloseMonth)}};
            var provider = new Mock<IPartialDisclosureListProvider>();
            provider.Setup(x => x.Execute()).Returns(stopList);
            var service = new PartialDisclosureService(provider.Object);

            // Act
            var attributes = new ProofOfTestAttributes(DateTime.UtcNow, "ZXY", "A", "B", "2", "1", true, true);
            var result = service.Apply(attributes);

            // Assert
            Assert.NotNull(result);
            Assert.NotSame(attributes, result);
            Assert.Equal("1", result.IsSpecimen);
            Assert.Equal("1", result.IsPaperProof);
            Assert.Equal(attributes.SampleTime, result.SampleTime);
            Assert.Equal("ZXY", result.TestType);

            // Assert: check the partial disclosures
            Assert.Equal(discloseFirstInitial ? "A" : string.Empty, result.FirstNameInitial);
            Assert.Equal(discloseLastInitial ? "B" : string.Empty, result.LastNameInitial);
            Assert.Equal(discloseMonth ? "1" : string.Empty, result.BirthMonth);
            Assert.Equal(discloseDay ? "2" : string.Empty, result.BirthDay);
        }

        [Fact]
        public void Apply_returns_attributes_unchanged_when_stoplist_not_matched()
        {
            // Assemble
            var stopList = new Dictionary<string, StopFilter>();
            var provider = new Mock<IPartialDisclosureListProvider>();
            provider.Setup(x => x.Execute()).Returns(stopList);
            var service = new PartialDisclosureService(provider.Object);

            // Act
            var attributes = new ProofOfTestAttributes(DateTime.UtcNow, "ZXY", "A", "B", "2", "1", true, true);
            var result = service.Apply(attributes);

            // Assert
            Assert.NotNull(result);
            Assert.Same(attributes, result);
            Assert.Equal("1", result.IsSpecimen);
            Assert.Equal("1", result.IsPaperProof);
            Assert.Equal(attributes.SampleTime, result.SampleTime);
            Assert.Equal("ZXY", result.TestType);

            // Assert: partial disclosure attributes unchanged
            Assert.NotNull(result);
            Assert.Same(attributes, result);
            Assert.Equal("1", result.BirthMonth);
            Assert.Equal("2", result.BirthDay);
            Assert.Equal("A", result.FirstNameInitial);
            Assert.Equal("B", result.LastNameInitial);
        }
    }
}
