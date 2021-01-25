// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Extensions
{
    public static class DateTimeExtensions
    {
        /// <summary>
        /// GO API string is based on ISO 1806 format but including only the hour (minutes/second are zeroed out).
        /// </summary>
        /// <example>2021-01-25T13:00:00.0000000Z</example>
        public static string ToGoApiString(this DateTime date)
        {
            return $"{date:yyyy-MM-dd}T{date:HH}:00:00.0000000Z";
        }
    }
}