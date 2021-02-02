// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Services
{
    /// <summary>
    /// Dummy implementation
    /// </summary>
    public class DummyProofOfTestService : IProofOfTestService
    {
        public string GetProofOfTest(string testType, string dateTime, string nonce, string commitments)
        {
            return Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(IssuerResponse));
        }
        
        public string GenerateNonce()
        {
            return "hgc3oMZzWd/rEcjdpHsNnw==";
        }

        private string IssuerResponse =
            "{\"proof\":{\"c\":\"kuyl1W2kEehxMcMDo1b4Lg71n5NdKsIK9/k60IxLJzA=\",\"e_response\":\"C0d/VZBY8gVf9OcTahjKZsTWmbX08IUulgO+noRAoc9bG73Vxf8ReOOLZy2dXk9nyYcXEK45R4HyIGyaIbAlHjweFbCznl0cdIPKtvgX7KiFFuVk+EgXoUjMe/OQ9JAKTM0YMZQMJyzdKLB28Uh7pSERMwDM8gJ8z2X9LLwkOPuvc8h7juWunwkcs+nLsi1uSyI9eCycI3Ud29oChIlJMGNswCWzJ0P821rbZuq4PfhstfAlLw0EeAZzeyTSz2xhEWWvD6lDiu8o0s8+ktua+P8yl9eK/WhJSzLsF9EnWeJqO8YJjTEBjorNw2vb9ayMAynffxHLeHBaaMq9sWLG0Q==\"},\"signature\":{\"A\":\"jTCnqj1/5Fvqzx584yJeZJs+A6NaHT/hhtsdf1eQifwv+DeLhtN2cbZMutAwX+/f1my+AB8KyuwNnq3UB3oJ4O3UtQRT2CGVofhDXg8svfxKjNbJ/AjM6pPKZwCGaJdcFAM3utD/QVz6yynEofDXznWaxaB20eijNR//SGvB8GPEiDBlwqzMjnwPDu6ytBbB05JOqCLH8EWdfntVjXOYezh9ygoslG0D5kOIK7W2g/sQUMW/gg6B+pg4ylrrgFRrIDisZCcrKavcq+0rBS95316gIAZkMRMk7mTjQ7Js7MxSHOUJS0pgUg4fzLuNaoP2hPYYZ/lW2IN3aMDwtjaKUg==\",\"e\":\"EAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAADDMO0xQSOgp6f+0kg4Mr\",\"v\":\"CvjnT54oNVZg7AL3jR0u9bYrvB7AE3QvMFlb6V1CJHEoc8e5YMwQ9uas1KHtpG7IGJGg4+wbMbeDQ3eQ9rmkonbaqLCqw21B8HHSEX6PrMvafVoMFHRWvLCz22lcQXd/vcO+XgAJhbresTRSVDcP9jJr0hP1wTY3y93xV89IlESHR8zlTUxyFfhTo/A8cy0ewTyvxJ6A74lcHdBcNtm0zw/QSuPRlpu3w9fQx+u9PQAR8mdVlHnI05UB4mt+2Is/03RGSMVFbwB74PvGEuaAZOi6ddlQULU89/47dak901DMYkHPH9a8afeHmw1DBXz3EwImF+PC1nTDhliW0GbmGcNbigFWEX8rBX8Hu7WkDG8hhag3awE1i1pD5yBxZ/U6yn9Td4gwZPGnF5T04CsdEF2IL2Vvu2AydAT+NdOy4S0dyN5bITAscc29xNldEYi82kJ02bg9ywx2aMslXaSiKOA=\",\"KeyshareP\":null}}";
    }
}