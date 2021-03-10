// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

#nullable enable
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Extensions;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using System;
using System.ComponentModel.DataAnnotations;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Validation
{
    public class DateRangeAttribute : ValidationAttribute
    {
        private const string ValidationError = "Value out of range";

        public int ValidityHours { get; set; }

        public DateRangeAttribute(int validityHours)
        {
            ValidityHours = validityHours;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult(ValidationError);

            var dateTimeProvider = GetDateTimeProvider(validationContext);
            var date = (DateTime) value;

            return date.LessThanNHoursBefore(ValidityHours, dateTimeProvider.Snapshot)
                ? ValidationResult.Success
                : new ValidationResult(ValidationError);
        }

        private static IUtcDateTimeProvider GetDateTimeProvider(ValidationContext validationContext)
        {
            var service = validationContext.GetService(typeof(IUtcDateTimeProvider));
            if (service == null) throw new InvalidOperationException("Service IUtcDateTimeProvider is not available.");
            return (IUtcDateTimeProvider) service;
        }
    }
}
