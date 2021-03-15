// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

#nullable enable
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Validation
{
    /// <summary>
    /// Validates that the string matches the structure of a base64 string
    /// </summary>
    public class Base64StringAttribute : ValidationAttribute
    {
        private const string ValidationError = "Input is not a valid base64 string";

        /// <summary>
        /// Matches the alphabet per page 6 of RFC 4648 (https://tools.ietf.org/html/rfc4648)
        /// </summary>
        private const string Base64StringPattern = @"^[a-zA-Z0-9=\+\/]+$";

        private readonly Regex _expression;

        public Base64StringAttribute()
        {
            _expression = new Regex(Base64StringPattern);
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext _)
        {
            if (value == null) return new ValidationResult(ValidationError);

            var valueString = (string)value;

            // Check length is divisible by 4
            if (valueString.Length % 4 != 0)
                return new ValidationResult(ValidationError);

            // Check against the regex
            if (!_expression.IsMatch(valueString))
                return new ValidationResult(ValidationError);

            return ValidationResult.Success;
        }
    }
}