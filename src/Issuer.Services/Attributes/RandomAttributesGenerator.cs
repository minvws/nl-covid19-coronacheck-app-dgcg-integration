// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Extensions;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Attributes
{
    public class RandomAttributesGenerator
    {
        private readonly List<string> _birthDays = new List<string>(32);
        private readonly List<string> _birthMonths = new List<string>(13);
        private readonly List<string> _initials = new List<string>(26);
        private readonly Random _rng = new Random();

        public RandomAttributesGenerator()
        {
            for (var i = 65; i < 91; i++)
            {
                _initials.Add(new string((char) i, 1));
            }

            _birthDays.Add("X");
            for (var i = 1; i < 32; i++)
            {
                _birthDays.Add(i.ToString());
            }

            _birthMonths.Add("X");
            for (var i = 1; i < 13; i++)
            {
                _birthMonths.Add(i.ToString());
            }
        }

        public ProofOfTestAttributes Generate()
        {
            var firstName = _initials[_rng.Next(0, _initials.Count - 1)];
            var lastName = _initials[_rng.Next(0, _initials.Count - 1)];
            var birthDay = _birthDays[_rng.Next(0, _birthDays.Count - 1)];
            var birthMonth = _birthMonths[_rng.Next(0, _birthMonths.Count - 1)];
            var sampleDate = DateTime.UtcNow.AddHours(-1 * _rng.Next(0, 71)).ToHourPrecision();

            return new ProofOfTestAttributes(sampleDate, "PCR", firstName, lastName, birthDay, birthMonth);
        }
    }
}
