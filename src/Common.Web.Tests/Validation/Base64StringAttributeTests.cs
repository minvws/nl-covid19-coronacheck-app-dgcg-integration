// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Validation;
using Xunit;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Tests.Validation
{
    public class Base64StringAttributeTests
    {
        [Theory]
        [InlineData("aGVsbG8gd29ybGQ=", true)]
        [InlineData("RnJpZW5kcywgUm9tYW5zLCBjb3VudHJ5bWVuLCBsZW5kIG1lIHlvdXIgZWFyczsgSSBjb21lIHRvIGJ1cnkgQ2Flc2FyLCBub3QgdG8gcHJhaXNlIGhpbS4=", true)]
        [InlineData("WU9VIHdpbGwgcmVqb2ljZSB0byBoZWFyIHRoYXQgbm8gZGlzYXN0ZXIgaGFzIGFjY29tcGFuaWVkIHRoZSBjb21tZW5jZW1lbnQgb2YgYW4gZW50ZXJwcmlzZSB3aGljaCB5b3UgaGF2ZSByZWdhcmRlZCB3aXRoIHN1Y2ggZXZpbCBmb3JlYm9kaW5ncy4gSSBhcnJpdmVkIGhlcmUgeWVzdGVyZGF5OyBhbmQgbXkgZmlyc3QgdGFzayBpcyB0byBhc3N1cmUgbXkgZGVhciBzaXN0ZXIgb2YgbXkgd2VsZmFyZSwgYW5kIGluY3JlYXNpbmcgY29uZmlkZW5jZSBpbiB0aGUgc3VjY2VzcyBvZiBteSB1bmRlcnRha2luZy4=", true)]
        [InlineData("aGVsbG8gd29ybGQ", false)]
        [InlineData("aaaGVsbG8gd29ybGQ", false)]
        [InlineData("aGVsb_G8gd29bGQ=", false)]
        [InlineData("aGVsbG%8gd29bGQ=", false)]
        public void ValidatesCorrectly(string base64String, bool expectedResult)
        {
            var attribute = new Base64StringAttribute();
            var result = attribute.IsValid(base64String);
            Assert.Equal(expectedResult, result);
        }
    }
}
