using HolderInterop;
using NL.Rijksoverheid.CoronaCheck.BackEnd.IssuerInterop;
using Xunit;

namespace HolderInteropTests
{
    public class HolderTests
    {
        [Fact]
        public void TestCreateCommitmentMessage()
        {
            // Assemble issuer
            var issuer = new Issuer();
            var nonce = issuer.GenerateNonce(Keys.TestIssuerPkId);

            // Assemble holder
            var holder = new Holder();
            holder.LoadIssuerPks(Keys.AnnotatedKeys());
            var secretKey = holder.GenerateHolderSecretKey();

            // Act
            var result = holder.CreateCommitmentMessage(secretKey, nonce);

            // Assert
            Assert.NotEmpty(result);
        }

        [Fact]
        public void TestGenerateHolderSecretKey()
        {
            var holder = new Holder();

            var result = holder.GenerateHolderSecretKey();

            Assert.NotEmpty(result);
        }

        [Fact]
        public void TestLoadIssuerPks()
        {
            // Assemble
            var holder = new Holder();

            // Act
            holder.LoadIssuerPks(Keys.AnnotatedKeys());
        }

        // TODO:RB: This isn't actually needed for now
        //[Fact]
        //public void TestCreateCredential()
        //{
        //    // Assemble issuer
        //    var issuer = new Issuer();
        //    var nonce = issuer.GenerateNonce(Keys.TestIssuerPkId);

        //    // Assemble holder
        //    var holder = new Holder();
        //    holder.LoadIssuerPks(Keys.AnnotatedKeys());
        //    var secretKey = holder.GenerateHolderSecretKey();
        //    var ccm = holder.CreateCommitmentMessage(secretKey, nonce);

        //    // Act
        //    var result = holder.CreateCredential(secretKey, ccm);

        //    // Assert
        //    Assert.NotEmpty(result);
        //}
    }
}
