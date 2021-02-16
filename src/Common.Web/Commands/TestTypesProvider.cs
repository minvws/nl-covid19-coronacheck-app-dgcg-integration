// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Common.Database.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using TestType = NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models.TestType;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Commands
{
    public class TestTypesProvider : ITestTypesProvider
    {
        private readonly Func<TesterContext> _contextFactory;

        public TestTypesProvider(Func<TesterContext> contextFactory)
        { 
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }
        
        public IList<TestType> Get()
        {
            using var ctx = _contextFactory();

            return ctx.TestTypes.Select(x => new TestType
            {
                Id = x.Uuid,
                Name = x.Name
            }).ToList();
        }
    }
}