// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Globalization;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Extensions;
using Xunit;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Tests.Extensions;

public class DateTimeExtensionsTests
{
    [Theory]
    [InlineData("2021-01-25T13:00:00.00Z", "2021-01-25T13:00:00.00Z", 72, true)]
    [InlineData("2021-01-24T13:00:00.00Z", "2021-01-25T13:00:00.00Z", 72, true)]
    [InlineData("2021-01-23T13:00:00.00Z", "2021-01-25T13:00:00.00Z", 72, true)]
    [InlineData("2021-01-22T13:00:00.00Z", "2021-01-25T13:00:00.00Z", 72, true)]
    [InlineData("2021-01-21T13:00:00.00Z", "2021-01-25T13:00:00.00Z", 72, false)]
    [InlineData("2021-01-22T12:00:00.00Z", "2021-01-25T13:00:00.00Z", 72, false)]
    [InlineData("2021-01-22T12:59:59.59Z", "2021-01-25T13:00:00.00Z", 72, false)]
    [InlineData("2021-01-25T13:00:01.00Z", "2021-01-25T13:00:00.00Z", 72, false)]
    public void LessThanNHoursBeforeExpectedResult(string dateString, string comparisonDateString, int hours, bool expectedResult)
    {
        var date = DateTime.Parse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        var comparisonDate = DateTime.Parse(comparisonDateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

        var result = date.LessThanNHoursBefore(hours, comparisonDate);

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("2021-01-25T00:00:00.00Z", true)]
    [InlineData("2021-01-25T13:00:00.00Z", true)]
    [InlineData("2021-01-25T13:10:00.00Z", false)]
    [InlineData("2021-01-25T13:00:10.00Z", false)]
    [InlineData("2021-01-25T13:00:00.01Z", false)]
    public void IsHourPrecisionExpectedResult(string dateString, bool expectedResult)
    {
        var date = DateTime.Parse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

        var result = date.IsHourPrecision();

        Assert.Equal(expectedResult, result);
    }

    [Theory]
    [InlineData("2021-01-25T13:10:00.00Z", "2021-01-25T13:00:00.00Z")]
    [InlineData("2021-01-25T13:00:30.00Z", "2021-01-25T13:00:00.00Z")]
    [InlineData("2021-01-25T13:00:00.30Z", "2021-01-25T13:00:00.00Z")]
    public void ToHourPrecisionExpectedResult(string dateString, string expectedDataString)
    {
        var date = DateTime.Parse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);
        var expectedDate = DateTime.Parse(expectedDataString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

        var result = date.ToHourPrecision();

        Assert.Equal(expectedDate, result);
    }

    [Theory]
    [InlineData("2021-01-25T00:00:00.00Z", 1611532800)]
    [InlineData("2021-01-25T13:00:00.00Z", 1611579600)]
    [InlineData("2021-01-25T13:10:00.00Z", 1611580200)]
    public void ToUnixTimeExpectedResult(string dateString, int expectedUnixTime)
    {
        var date = DateTime.Parse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

        var result = date.ToUnixTime();

        Assert.Equal(expectedUnixTime, result);
    }
}
