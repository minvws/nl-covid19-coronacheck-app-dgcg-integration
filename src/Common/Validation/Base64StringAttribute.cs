// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

#nullable enable
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Validation
{
    public class Base64StringAttribute : ValidationAttribute
    {
        private const string ValidationError = "Input is not a valid base64 string";

        /// <summary>
        /// Matches the alphabet per page 6 of RFC 4648 (https://tools.ietf.org/html/rfc4648)
        /// </summary>
        private const string Base64StringPattern = @"[a-zA-Z0-9=\+\/]+";
        
        private readonly Regex _expression;

        public Base64StringAttribute()
        {
            _expression = new Regex(Base64StringPattern);
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null) return base.IsValid(value, validationContext);

            var valueString = (string) value;
            
            return _expression.IsMatch(valueString) ? ValidationResult.Success : new ValidationResult(ValidationError);
        }
    }
}