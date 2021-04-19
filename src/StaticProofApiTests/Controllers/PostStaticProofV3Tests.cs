// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System.Threading.Tasks;
using Xunit;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.StaticProofApiTests.Controllers
{
    /// <summary>
    /// </summary>
    public class PostStaticProofV3Tests : PostStaticProofTestsBase
    {
        protected override string ApiVersion => "v3";

        [Theory]
        [InlineData(true, -60)]
        [InlineData(true, -1440)]
        [InlineData(false, 60)]
        [InlineData(false, -10)]
        [InlineData(false, -42)]
        public async Task Test_sample_dates(bool success, double sampleDateOffsetMinutes)
        {
            var result = await ExecuteTest(sampleDateOffsetMinutes);
            Assert.Equal(success, result);
        }

        [Theory]
        [InlineData(true, "1")]
        [InlineData(true, "31")]
        [InlineData(true, "X")]
        [InlineData(true, "x")]
        [InlineData(false, "")]
        [InlineData(false, "32")]
        [InlineData(false, "A")]
        [InlineData(false, "%")]
        [InlineData(false, "-1")]
        [InlineData(false, "1.32")]
        public async Task Test_birth_day(bool success, string birthDay)
        {
            var result = await ExecuteTest(0, birthDay);
            Assert.Equal(success, result);
        }

        [Theory]
        [InlineData(true, "1")]
        [InlineData(true, "12")]
        [InlineData(true, "X")]
        [InlineData(true, "x")]
        [InlineData(false, "")]
        [InlineData(false, "32")]
        [InlineData(false, "A")]
        [InlineData(false, "%")]
        [InlineData(false, "-1")]
        [InlineData(false, "1.32")]
        public async Task Test_birth_month(bool success, string birthMonth)
        {
            var result = await ExecuteTest(0, birthMonth);
            Assert.Equal(success, result);
        }

        [Theory]
        [InlineData(true, "A")]
        [InlineData(true, "Z")]
        [InlineData(false, "a")]
        [InlineData(false, "z")]
        [InlineData(false, "")]
        [InlineData(false, "1")]
        [InlineData(false, "1.1")]
        [InlineData(false, "-1")]
        [InlineData(false, "%")]
        [InlineData(false, "AA")]
        public async Task Test_initials(bool success, string initial)
        {
            var result = await ExecuteTest(0, "1", "1", initial);
            Assert.Equal(success, result);
            result = await ExecuteTest(0, "1", "1", "A", initial);
            Assert.Equal(success, result);
        }
    }
}
