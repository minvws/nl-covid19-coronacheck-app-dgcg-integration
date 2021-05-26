//using System;
//using System.Linq;
//using System.Security.Cryptography.X509Certificates;
//using Org.BouncyCastle.Cms;
//using Org.BouncyCastle.Security;

//namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Signing
//{
//    public static class SignatureVerifiers
//    {
//        public static bool ValidateCmsWithBouncy(X509Certificate2 certificate, byte[] signature, byte[] content)
//        {
//            if (certificate == null) throw new ArgumentNullException(nameof(certificate));
//            if (content == null) throw new ArgumentNullException(nameof(content));
//            if (signature == null) throw new ArgumentNullException(nameof(signature));

//            try
//            {
//                var bouncyCertificate = DotNetUtilities.FromX509Certificate(certificate);
//                var publicKey = bouncyCertificate.GetPublicKey();
//                var cms = new CmsSignedData(new CmsProcessableByteArray(content), signature);

//                var result = cms.GetSignerInfos()
//                                .GetSigners()
//                                .Cast<SignerInformation>()
//                                .Select(signer => signer.Verify(publicKey))
//                                .FirstOrDefault();

//                return result;
//            }
//            catch (Exception e)
//            {

//                return false;
//            }
//        }
//    }
//}


