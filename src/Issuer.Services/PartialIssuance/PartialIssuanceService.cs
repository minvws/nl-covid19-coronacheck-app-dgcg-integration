// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Attributes;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.PartialIssuance
{
    public class PartialIssuanceService : IPartialIssuanceService
    {
        private readonly IReadOnlyDictionary<string, WhitelistItem> _list;

        public PartialIssuanceService(IReadOnlyDictionary<string, WhitelistItem> list)
        {
            _list = list ?? throw new ArgumentNullException(nameof(list));
        }

        public PartialIssuanceService(IWhitelistProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            _list = provider.Execute();
        }

        public ProofOfTestAttributes Apply(ProofOfTestAttributes attributes)
        {
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));

            var search = attributes.FirstNameInitial + attributes.LastNameInitial;

            if (!_list.ContainsKey(search)) return attributes;

            var whitelist = _list[search];

            return new ProofOfTestAttributes
            {
                SampleTime = attributes.SampleTime,
                TestType = attributes.TestType,
                FirstNameInitial = whitelist.AllowFirstInitial ? attributes.FirstNameInitial : string.Empty,
                LastNameInitial = whitelist.AllowLastInitial ? attributes.LastNameInitial : string.Empty,
                BirthMonth = whitelist.AllowMonth ? attributes.BirthMonth : string.Empty,
                BirthDay = whitelist.AllowDay ? attributes.BirthDay : string.Empty,
                IsPaperProof = attributes.IsPaperProof,
                IsSpecimen = attributes.IsSpecimen
            };
        }
    }
}
