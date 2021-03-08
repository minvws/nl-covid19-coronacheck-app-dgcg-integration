// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Moq;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Testing;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Xunit;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Tests.Validation
{
    public class DateRangeAttributeTests
    {
        [Theory]
        [InlineData("2021-01-25T13:00:00.00Z", "2021-01-25T13:00:00.00Z", true)]
        [InlineData("2021-01-21T13:00:00.00Z", "2021-01-25T13:00:00.00Z", false)]
        public void ValidatesCorrectly(string nowDateString, string valueDateString, bool expectedResult)
        {
            // Assemble: date-time provider with given now
            var dateNow = DateTime.Parse(nowDateString, CultureInfo.InvariantCulture).ToUniversalTime();
            var dateTimeProvider = new TestUtcDateTimeProvider(dateNow);
            var mockProvider = new Mock<IServiceProvider>();
            mockProvider.Setup(x => x.GetService(It.IsAny<System.Type>())).Returns(dateTimeProvider);
            var serviceProvider = mockProvider.Object;

            // Assemble: model to validate
            var model = new TestModel
            {
                Test = DateTime.Parse(valueDateString, CultureInfo.InvariantCulture).ToUniversalTime()
            };

            // Assemble: bananas
            var context = new ValidationContext(model, serviceProvider, null);
            var results = new List<ValidationResult>();

            // Act
            var result = Validator.TryValidateObject(model, context, results, true);
            
            // Assert
            Assert.Equal(expectedResult, result);
        }
    }

    public class TestModel
    {
        [DateRangeAttribute(72)]
        public DateTime Test { get; set; }
    }
}
