// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Commands
{
    public class HardcodedTestTypesProvider : ITestTypesProvider
    {
        public IList<TestType> Get()
        {
            return new List<TestType>
            {
                new TestType {Id = Guid.NewGuid(), Name = "PCR"},
                new TestType {Id = Guid.NewGuid(), Name = "Snel"},
                new TestType {Id = Guid.NewGuid(), Name = "Adem"},
                new TestType {Id = Guid.NewGuid(), Name = "Antistoffen"}
            };
        }
    }
}