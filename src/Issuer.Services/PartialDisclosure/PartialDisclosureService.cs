// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.Attributes;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Issuer.Services.PartialDisclosure
{
    public class PartialDisclosureService : IPartialDisclosureService
    {
        private readonly IReadOnlyDictionary<string, StopFilter> _list;

        public PartialDisclosureService(IReadOnlyDictionary<string, StopFilter> list)
        {
            _list = list ?? throw new ArgumentNullException(nameof(list));
        }

        public PartialDisclosureService(IPartialDisclosureListProvider provider)
        {
            if (provider == null) throw new ArgumentNullException(nameof(provider));

            _list = provider.Execute();
        }

        public ProofOfTestAttributes Apply(ProofOfTestAttributes attributes)
        {
            if (attributes == null) throw new ArgumentNullException(nameof(attributes));

            var search = attributes.FirstNameInitial + attributes.LastNameInitial;

            if (!_list.ContainsKey(search)) return attributes;

            var stopFilter = _list[search];

            return new ProofOfTestAttributes
            {
                SampleTime = attributes.SampleTime,
                TestType = attributes.TestType,
                FirstNameInitial = stopFilter.DiscloseFirstInitial ? attributes.FirstNameInitial : string.Empty,
                LastNameInitial = stopFilter.DiscloseLastInitial ? attributes.LastNameInitial : string.Empty,
                BirthMonth = stopFilter.DiscloseMonth ? attributes.BirthMonth : string.Empty,
                BirthDay = stopFilter.DiscloseDay ? attributes.BirthDay : string.Empty,
                IsPaperProof = attributes.IsPaperProof,
                IsSpecimen = attributes.IsSpecimen
            };
        }
    }
}
