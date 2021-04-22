// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.PartialIssuance
{
    public static class WhitelistLoader
    {
        public static IReadOnlyDictionary<string, WhitelistItem> LoadFromFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentNullException(nameof(path));

            var lines = File.ReadAllLines(path);

            return Load(lines);
        }

        public static IReadOnlyDictionary<string, WhitelistItem> LoadFromString(string data)
        {
            if (string.IsNullOrWhiteSpace(data)) throw new ArgumentNullException(nameof(data));

            return Load(data.Contains("\r\n") ? data.Split("\r\n") : data.Split("\n"));
        }

        private static IReadOnlyDictionary<string, WhitelistItem> Load(string[] lines)
        {
            // Note: lines is not null here; if it's an empty array that's also OK

            var keyPattern = new Regex("^[A-Z]{2}$");
            var valuePattern = new Regex("^[VFMD]{1,4}$");

            var filter = new Dictionary<string, WhitelistItem>();

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;

                var split = line.Split(",");

                if (split.Length != 2) throw new FormatException($"Expected two lines, got {split.Length}.");

                var key = split[0];
                if (!keyPattern.IsMatch(key)) throw new FormatException($"Expected key to be two characters from A-Z, got: {key}.");

                var value = split[1];
                if (!valuePattern.IsMatch(value)) throw new FormatException($"Expected value to be 1-4 characters from the set [VFMD], got: {key}.");

                var discloseFirstInitial = value.Contains("V");
                var discloseLastInitial = value.Contains("F");
                var discloseMonth = value.Contains("M");
                var discloseDay = value.Contains("D");

                filter.Add(key, new WhitelistItem(discloseFirstInitial, discloseLastInitial, discloseDay, discloseMonth));
            }

            return filter;
        }
    }
}
