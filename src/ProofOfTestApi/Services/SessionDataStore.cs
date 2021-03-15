// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using Microsoft.AspNetCore.Http;
using System;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.ProofOfTestApi.Services
{
    public class SessionDataStore : ISessionDataStore
    {
        private const string NonceKey = "NONCE";

        private readonly IHttpContextAccessor _httpContextAccessor;

        private ISession Session
        {
            get
            {
                if (_httpContextAccessor.HttpContext == null)
                    throw new InvalidOperationException("HTTP Context unexpectedly null.");

                return _httpContextAccessor.HttpContext.Session;
            }
        }

        public SessionDataStore(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public string GetNonce()
        {
            return Session.GetString(NonceKey);
        }

        public void AddNonce(string nonce)
        {
            Session.Set(NonceKey, System.Text.Encoding.UTF8.GetBytes(nonce));
        }

        public void RemoveNonce()
        {
            Session.Remove(NonceKey);
        }
    }
}