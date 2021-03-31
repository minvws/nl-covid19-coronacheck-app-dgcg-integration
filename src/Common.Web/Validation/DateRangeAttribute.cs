// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.ComponentModel.DataAnnotations;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Extensions;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Validation
{
    public class DateRangeAttribute : ValidationAttribute
    {
        private const string ValidationError = "Value out of range";
        private readonly int _validityHours;

        public DateRangeAttribute(int validityHours)
        {
            if (validityHours < 0) throw new ArgumentException("Validity must be greater than 0", nameof(validityHours));

            _validityHours = validityHours;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return new ValidationResult(ValidationError);
            if (validationContext == null) throw new ArgumentNullException(nameof(validationContext));

            var dateTimeProvider = GetDateTimeProvider(validationContext);
            var date = (DateTime) value;

            return date.LessThanNHoursBefore(_validityHours, dateTimeProvider.Snapshot)
                ? ValidationResult.Success
                : new ValidationResult(ValidationError);
        }

        private static IUtcDateTimeProvider GetDateTimeProvider(ValidationContext validationContext)
        {
            if (validationContext == null) throw new ArgumentNullException(nameof(validationContext));

            var service = validationContext.GetService(typeof(IUtcDateTimeProvider));
            if (service == null) throw new InvalidOperationException("Service IUtcDateTimeProvider is not available.");
            return (IUtcDateTimeProvider) service;
        }
    }
}
