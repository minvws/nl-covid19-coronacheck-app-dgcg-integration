// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Certificates;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Validator;
using Xunit;

namespace DigitalGreenGatewayToolTests
{
    /// <summary>
    ///     These are integration tests, they require a local DGCG server configured with only our own keys as per
    ///     the guide from readme of the DGCG.
    /// </summary>
    public class TrustListValidatorTests
    {
        [Fact]
        public void Validate_supports_multiple_CSCA_from_one_country()
        {
            // Assemble
            var serializer = new StandardJsonSerializer();
            var trustList = serializer.Deserialize<List<TrustListItem>>(File.ReadAllText("TrustListExample.json"));
            var validator = new TrustListValidator(new TrustAnchorProvider());

            // Act
            var result = validator.Validate(trustList);

            // Assert
            Assert.Empty(result.InvalidItems);
        }
    }

    internal class TrustAnchorProvider : ICertificateProvider
    {
        private readonly string _cert = @"-----BEGIN CERTIFICATE-----
MIIGGzCCBAOgAwIBAgIUX3fFmZobc4FKykeIP3414BhGUdAwDQYJKoZIhvcNAQEL
BQAwgZwxCzAJBgNVBAYTAkRFMQ8wDQYDVQQIDAZIZXNzZW4xGjAYBgNVBAcMEUZy
YW5rZnVydCBhbSBNYWluMSUwIwYDVQQKDBxULVN5c3RlbXMgSW50ZXJuYXRpb25h
bCBHbWJIMRowGAYDVQQLDBFEaWdpdGFsIFNvbHV0aW9uczEdMBsGA1UEAwwUREdD
RyBUcnVzdEFuY2hvciBBQ0MwHhcNMjIwMzE3MTUwMzIzWhcNMjMwNDMwMTUwMzIz
WjCBnDELMAkGA1UEBhMCREUxDzANBgNVBAgMBkhlc3NlbjEaMBgGA1UEBwwRRnJh
bmtmdXJ0IGFtIE1haW4xJTAjBgNVBAoMHFQtU3lzdGVtcyBJbnRlcm5hdGlvbmFs
IEdtYkgxGjAYBgNVBAsMEURpZ2l0YWwgU29sdXRpb25zMR0wGwYDVQQDDBRER0NH
IFRydXN0QW5jaG9yIEFDQzCCAiIwDQYJKoZIhvcNAQEBBQADggIPADCCAgoCggIB
AKFq6WlAoSDL2tFEDc3h4Wdiee/acMHEf/135xRzON77/13MT9rmQIFROqI2pVhK
8Cb6BBlCZSz9OChHL/H4sre10H4KLrtLcQzmVPZTfPCF/xWCbX1iw0k3VZcPKuAx
MnqnvVh90n7JAKKVa2bkuehqmEvltRPoE6PVCHekM5XIchTGHll+kJopcvJMUwtr
IZAzQyR3vslMxR93YsJajvVktVvyAMBGsAK0Ux55nZXv39EO8Ntj+0TrEdqpHp8L
rL4FOQsVl90MX+uKTlVkKlvAYyAq+00nL5hsW4l/ONRSFTdvhd/VXNiEQqU/pg19
D5sLtsf6gwvRFkDjC62vpd8CTldt3JAe+z/yH6eKZJlWfdhUFWwcTgI5cGCkZLh2
ohXTIN8hwGVFxgdwom/NsLDhFTVX7trITK8G9jyxjzVxCGvz7G+k4Pw7yny/ILl4
3NnVsE6GbjFn8Qcdk9DTvtUlILJ1B2vU3OTsPD0Dc4vaITK1Fqpb2umFqSNwSmvm
XuPR4oTZGClT7FMVXkOkuMDK//pqDISVRQ3i9e5BbVOGqCOfWOEfB6Lehxnsaonv
owLHCnXoF6UEpTgjEJ/F4z5ATGbD31PzuvuuXhDCFkgARo/SOVJFS3ReUooSPu/5
9gB1d6dW4ABC5Tn6xPuyhPE44tHroAJoZXRYp0pnFOQpAgMBAAGjUzBRMB0GA1Ud
DgQWBBQAferjDYm1f4RnoFYcDY5WGzUiLDAfBgNVHSMEGDAWgBQAferjDYm1f4Rn
oFYcDY5WGzUiLDAPBgNVHRMBAf8EBTADAQH/MA0GCSqGSIb3DQEBCwUAA4ICAQBg
NjezZ3trAjN2vAD3aSey5xeX/0r7cGairIX+wVug9dozF1/e6GAijEs79RKOkRJJ
0mCn/ryBxDNf5qWuy6yxfct7RzUV2S9hRBqjUWnmv6et2EQoF1fKhdIAyCrwc7mp
9DU7X4+XLY7qbUIO12iPSwmxcaG3ncoWj1V8Z33foWaIqf+w7v7Sr6K6eNPC0DBj
6bOYI7wPwJbZSqs6Z0cj1n3+h4cwYDho369FXy+UwIXWYK+Vr3CiK4I0lHR1XDai
hbMkYZf1ObOlXjzK5fWfcSjVWvs8JbQmzHohhyJm46UkCuxJPRuAWIPeegJpjahp
QgDzgm1ay2xmUvtHj//hbHERWPbKBvdENsoh4iIZjnsG2+PLYcO9H14xsErQpSRX
flpQa6UWvwWMaNi25mDUXjhV2MTCIxY0xajQEhgkJvq+EwIztd+d23dvK77M280U
W+hzvtfj3coOkhVAAphFBdbese1JiZa5+UXQmQuefLMrADOdah9B1ZqcparF+lHe
wDzTvLYCJj7+eVbs0Xa+wuwMmbfiFzL2zHMJjcix9KWxz/VzoK5QAYfYLS5WaGpq
C0+bLdamk7WQXOr92edKwYksS8F7TDGMI9xT9SIzZawlVz4gpjFqFfwvl5eE5GVU
TG4tE33k/UnSMFC5zZJXebD6IFrw4B3dTwlt6cUHbA==
-----END CERTIFICATE-----";

        public X509Certificate2 GetCertificate()
        {
            return new X509Certificate2(Encoding.Default.GetBytes(_cert), "", X509KeyStorageFlags.Exportable);
        }
    }
}
