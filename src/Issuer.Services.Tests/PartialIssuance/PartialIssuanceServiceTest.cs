// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using Moq;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Extensions;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Attributes;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.PartialIssuance;
using Xunit;

namespace Issuer.Services.Tests.PartialIssuance
{
    public class PartialIssuanceServiceTest
    {
        [Fact]
        public void Apply_returns_new_attributes_on_match()
        {
            // Assemble
            var stopList = new Dictionary<string, WhitelistItem> {{"AB", new WhitelistItem(true, true, true, true)}};
            var provider = new Mock<IWhitelistProvider>();
            provider.Setup(x => x.Execute()).Returns(stopList);
            var service = new PartialIssuanceService(provider.Object);

            // Act
            var attributes = new ProofOfTestAttributes
            {
                SampleTime = DateTime.UtcNow.ToHourPrecision().ToUnixTime().ToString(),
                TestType = "ZXY",
                FirstNameInitial = "A",
                LastNameInitial = "B",
                BirthMonth = "2",
                BirthDay = "1",
                IsSpecimen = "1",
                IsPaperProof = "1"
            };
            var result = service.Apply(attributes);

            // Assert
            Assert.NotNull(result);
            Assert.NotSame(attributes, result);
        }

        [Fact]
        public void Apply_matches_only_one_item()
        {
            // Assemble
            var stopList = new Dictionary<string, WhitelistItem>
            {
                {"AA", new WhitelistItem(true, true, true, true)},
                {"AB", new WhitelistItem(false, false, false, false)}
            };
            var provider = new Mock<IWhitelistProvider>();
            provider.Setup(x => x.Execute()).Returns(stopList);
            var service = new PartialIssuanceService(provider.Object);

            // Act
            var attributes = new ProofOfTestAttributes
            {
                SampleTime = DateTime.UtcNow.ToHourPrecision().ToUnixTime().ToString(),
                TestType = "ZXY",
                FirstNameInitial = "A",
                LastNameInitial = "B",
                BirthMonth = "2",
                BirthDay = "1",
                IsSpecimen = "1",
                IsPaperProof = "1"
            };
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
        public void Apply_executes_expected_partial_issuances(bool allowFirstInitial, bool allowLastInitial, bool allowDay, bool allowMonth)
        {
            // Assemble
            var stopList = new Dictionary<string, WhitelistItem>
                {{"AB", new WhitelistItem(allowFirstInitial, allowLastInitial, allowDay, allowMonth)}};
            var provider = new Mock<IWhitelistProvider>();
            provider.Setup(x => x.Execute()).Returns(stopList);
            var service = new PartialIssuanceService(provider.Object);

            // Act
            var attributes = new ProofOfTestAttributes
            {
                SampleTime = DateTime.UtcNow.ToHourPrecision().ToUnixTime().ToString(),
                TestType = "ZXY",
                FirstNameInitial = "A",
                LastNameInitial = "B",
                BirthMonth = "1",
                BirthDay = "2",
                IsSpecimen = "1",
                IsPaperProof = "1"
            };
            var result = service.Apply(attributes);

            // Assert
            Assert.NotNull(result);
            Assert.NotSame(attributes, result);
            Assert.Equal("1", result.IsSpecimen);
            Assert.Equal("1", result.IsPaperProof);
            Assert.Equal(attributes.SampleTime, result.SampleTime);
            Assert.Equal("ZXY", result.TestType);

            // Assert: check the attributes issued
            Assert.Equal(allowFirstInitial ? "A" : string.Empty, result.FirstNameInitial);
            Assert.Equal(allowLastInitial ? "B" : string.Empty, result.LastNameInitial);
            Assert.Equal(allowMonth ? "1" : string.Empty, result.BirthMonth);
            Assert.Equal(allowDay ? "2" : string.Empty, result.BirthDay);
        }

        [Fact]
        public void Apply_returns_attributes_unchanged_when_whitelist_not_matched()
        {
            // Assemble
            var stopList = new Dictionary<string, WhitelistItem>();
            var provider = new Mock<IWhitelistProvider>();
            provider.Setup(x => x.Execute()).Returns(stopList);
            var service = new PartialIssuanceService(provider.Object);

            // Act
            var attributes = new ProofOfTestAttributes
            {
                SampleTime = DateTime.UtcNow.ToHourPrecision().ToUnixTime().ToString(),
                TestType = "ZXY",
                FirstNameInitial = "A",
                LastNameInitial = "B",
                BirthMonth = "1",
                BirthDay = "2",
                IsSpecimen = "1",
                IsPaperProof = "1"
            };
            var result = service.Apply(attributes);

            // Assert
            Assert.NotNull(result);
            Assert.Same(attributes, result);
            Assert.Equal("1", result.IsSpecimen);
            Assert.Equal("1", result.IsPaperProof);
            Assert.Equal(attributes.SampleTime, result.SampleTime);
            Assert.Equal("ZXY", result.TestType);

            // Assert: attributes unchanged
            Assert.NotNull(result);
            Assert.Same(attributes, result);
            Assert.Equal("1", result.BirthMonth);
            Assert.Equal("2", result.BirthDay);
            Assert.Equal("A", result.FirstNameInitial);
            Assert.Equal("B", result.LastNameInitial);
        }
    }
}
