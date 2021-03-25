// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Validation;
using System;
using System.Globalization;
using Xunit;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Tests.Validation
{
    public class DatePrecisionAttributeTests
    {
        [Theory]
        [InlineData("2021-01-25T00:00:00.00Z", true)]
        [InlineData("2021-01-25T13:00:00.00Z", true)]
        [InlineData("2021-01-25T13:10:00.00Z", false)]
        [InlineData("2021-01-25T13:00:10.00Z", false)]
        [InlineData("2021-01-25T13:00:00.01Z", false)]
        public void HourPrecisionValidatesCorrectly(string dateString, bool expectedResult)
        {
            var date = DateTime.Parse(dateString, CultureInfo.InvariantCulture).ToUniversalTime();
            var attribute = new DatePrecisionAttribute { Precision = PrecisionLevel.Hour };

            var result = attribute.IsValid(date);

            Assert.Equal(expectedResult, result);
        }
    }
}
