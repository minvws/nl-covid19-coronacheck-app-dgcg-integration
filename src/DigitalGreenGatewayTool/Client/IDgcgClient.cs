﻿// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Collections.Generic;
using System.Threading.Tasks;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client
{
    public interface IDgcgClient
    {
        Task<IReadOnlyList<TrustListItem>> GetTrustList();
        Task<IReadOnlyList<TrustListItem>> GetTrustList(CertificateType type);
        Task<IReadOnlyList<TrustListItem>> GetTrustList(CertificateType type, string country);
        Task<bool> Upload(byte[] certificateBytes);
        Task<bool> Revoke(byte[] certificateBytes);
    }
}
