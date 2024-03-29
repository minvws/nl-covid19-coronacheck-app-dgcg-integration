﻿// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Collections.Generic;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Validator
{
    public class TrustListValidatorResult
    {
        private readonly List<TrustListItem> _invalidItems = new List<TrustListItem>();
        private readonly Dictionary<TrustListItem, string> _invalidReason = new Dictionary<TrustListItem, string>();
        private readonly List<TrustListItem> _validItems = new List<TrustListItem>();

        public IReadOnlyList<TrustListItem> ValidItems => _validItems;

        public IReadOnlyList<TrustListItem> InvalidItems => _invalidItems;

        public void AddValid(TrustListItem item)
        {
            _validItems.Add(item);
        }

        public void AddInvalid(TrustListItem item, string reason)
        {
            _invalidItems.Add(item);
            _invalidReason[item] = reason;
        }

        public string GetReasonInvalid(TrustListItem item)
        {
            return _invalidReason.ContainsKey(item) ? _invalidReason[item] : string.Empty;
        }
    }
}
