//using System;
//using System.Security.Cryptography.X509Certificates;
//using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Signing;
//using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client;

//namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Validator
//{
//    public interface ITrustListItemValidator
//    {
//        bool Validate(TrustListDto item, X509Certificate2 trustCertificate);
//    }

//    public class TrustListItemValidator : ITrustListItemValidator
//    {
//        private readonly ICmsValidator _cmsValidator;

//        public TrustListItemValidator(ICmsValidator cmsValidator)
//        {
//            _cmsValidator = cmsValidator ?? throw new ArgumentNullException(nameof(cmsValidator));
//        }

//        public bool Validate(TrustListDto item, X509Certificate2 trustCertificate)
//        {
//            var certificateBytes = Convert.FromBase64CharArray(item.RawData);
//            var signatureBytes = Convert.FromBase64CharArray(item.Signature);

//            return _cmsValidator.ValidateWith(certificateBytes, signatureBytes, trustCertificate);
//        }
//    }
//}


