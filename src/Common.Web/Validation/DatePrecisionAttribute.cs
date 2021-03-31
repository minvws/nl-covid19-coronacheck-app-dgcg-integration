// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.ComponentModel.DataAnnotations;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Extensions;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Web.Validation
{
    public class DatePrecisionAttribute : ValidationAttribute
    {
        private const string ValidationError = "Incorrect precision";

        public PrecisionLevel Precision { get; set; }

        protected override ValidationResult? IsValid(object? value, ValidationContext _)
        {
            if (_ == null) throw new ArgumentNullException(nameof(_));
            if (value == null) return new ValidationResult(ValidationError);

            var date = (DateTime) value;

            return Precision switch
            {
                PrecisionLevel.Hour => date.IsHourPrecision()
                    ? ValidationResult.Success
                    : new ValidationResult(ValidationError),
                _ => new ValidationResult(ValidationError)
            };
        }
    }
}
