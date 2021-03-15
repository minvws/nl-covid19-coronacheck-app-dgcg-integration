// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Common.Database.Model;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models;
using System;
using System.Linq;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Commands
{
    public class AppConfigProvider : IAppConfigProvider
    {
        private readonly Func<TesterContext> _contextFactory;
        private readonly IJsonSerializer _serializer;

        public AppConfigProvider(Func<TesterContext> contextFactory, IJsonSerializer serializer)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
        }

        public AppConfigResult Get(string type)
        {
            if (string.IsNullOrWhiteSpace(type)) throw new ArgumentNullException(nameof(type));

            using var ctx = _contextFactory();

            var item = ctx.AppConfigs.FirstOrDefault(_ => _.Type == type);

            if (item == null) throw new InvalidOperationException($"Could not find appconfig of type: {type}");

            return _serializer.Deserialize<AppConfigResult>(item.Content);
        }
    }
}