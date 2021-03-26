// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using Moq;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Validation;
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
            // Assemble: model to validate
            var date = DateTime.Parse(dateString, CultureInfo.InvariantCulture).ToUniversalTime();
            var model = new TestModel
            {
                Test = date
            };

            // Assemble: validator arguments
            var mockProvider = new Mock<IServiceProvider>();
            var context = new ValidationContext(model, mockProvider.Object, null);
            var results = new List<ValidationResult>();

            // Act
            var result = Validator.TryValidateObject(model, context, results, true);

            // Assert
            Assert.Equal(expectedResult, result);
        }

        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class TestModel
        {
            [DatePrecision(Precision = PrecisionLevel.Hour)]
            public DateTime Test { get; set; }
        }
    }
}
