// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Extensions;
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

            var sampleTime = DateTimeOffset.FromUnixTimeSeconds(int.Parse(attributes.SampleTime)).UtcDateTime;

            // Note: These Neither of these two exceptions can happen without some serious changes to ProofOfTestAttributes.
            //       I'm leaving the checks in because it's not impossible.
            var isPaperProof = attributes.IsPaperProof.ToBooleanFromIntegerExceptionOnFail();
            var isSpecimen = attributes.IsSpecimen.ToBooleanFromIntegerExceptionOnFail();

            return new ProofOfTestAttributes(
                sampleTime,
                attributes.TestType,
                stopFilter.DiscloseFirstInitial ? attributes.FirstNameInitial : string.Empty,
                stopFilter.DiscloseLastInitial ? attributes.LastNameInitial : string.Empty,
                stopFilter.DiscloseDay ? attributes.BirthDay : string.Empty,
                stopFilter.DiscloseMonth ? attributes.BirthMonth : string.Empty,
                isPaperProof,
                isSpecimen);
        }
    }
}
