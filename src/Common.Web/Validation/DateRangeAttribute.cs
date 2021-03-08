// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

#nullable enable
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Extensions;
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

        protected override ValidationResult? IsValid(object? value, ValidationContext _)
        {
            if (value == null) return new ValidationResult(ValidationError);

            var now = DateTime.Now;
            var date = (DateTime)value;

            return date.LessThanNHoursBefore(ValidityHours, now) ? ValidationResult.Success : new ValidationResult(ValidationError);
        }
    }
}
