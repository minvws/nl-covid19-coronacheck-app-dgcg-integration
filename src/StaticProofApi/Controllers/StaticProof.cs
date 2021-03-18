using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NL.Rijksoverheid.CoronaTester.BackEnd.Common.Web.Models;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.StaticProofApi.Controllers
{
    [ApiController]
    [Route("staticproof")]
    public class StaticProof : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<StaticProof> _logger;

        public StaticProof(ILogger<StaticProof> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        [Route("paper")]
        public PaperResponse Paper(SignedDataWrapper<object> request)
        {
            return new PaperResponse
            {
                Error = 0,
                Status = "ok",
                Qr = new QrCode
                {
                    Data =
                        @"12GRXVIHJXJVRGQ8IG%G/O9DA4+MQ9Q%9/:.MILS+QX-KLWI6N46TQT37IMPGQF-:6%$/GFW2UDLVOCY7W5GNICDEK33%MMWO%SQSO%RQLQ.RNN P7$U839RBG/$2O4PB90+ FOE2QV$CNKU8K5N10M1:06$4Z2J-B7-89X0-QXD.H:LK33A/JY:-8+WV2BI5Q6YJ97SR8VGKBGX3X7$KP*QAWDLKRJCWPT0P*B9PWA9IT*RNTL5TX$536Y V/5:JKTM.R4WW-0/$3HP2U30AB7RHWK8ZED48 6KWAC1YXEP480A.LTZ7F6G$:EGNE.S27S.$84FO/BY9INTV%0I/TSL-6O5TBWLL%AZWVHH7:ZS*3W WDCPEMFBHA2331SB1U S1XT$%MBA5%6-8DPRV.5 RTSOW+4V7E.$T8TU*:T:W$$VBP5F6$P+ 8QLYZGY$Z6QKD6ZQ7F.BC7451IG95YF9IWA6BTTYMV3U6O$JX5QOK/94 M/+J0W1QSDGXW15B/Y%J4D4 0UIJR/D9: NVCR9WGFKYRF1JWS$9H5BN3XPP92V71E J$/5ZM/3R XTLDK3C9OSK4RUXG56T/6EHO9A$8G*4VC*./TK60HOW*7J46XK-UAEG1M+O15$1CKA3X1NXDNF-ADU PT42*ZAEBVZ59K:.9Z40VTAW/1% I6Q5LG$7KBDP7C-KXGXE+BXT 0488LGF641R9B%ZCB:I6OX.Z%:0YWUVB8I0/N..H$*ZDZOO0U*FA6BE+BYHBHGU58R++AS/X8P6JGMNNJU0%*T1*J49I%DHLRA89NWBW6L59OWWVMLJ4EZ*C1BGY-HDQ+4QMAIBD:.04+DX5GO 3X3R:7-99K5VS$7YUVTOI7V TAGB9S+26F+$8N-YUT2/93CABR-2N3YHVQG6M:M7SBXV%9D0.U.2A0D7Z0ANVB9JA5YL14LB8P90Y:27V.0QJ3:E2+A30V%IY.YW.MK6QTPC%9RM7+KT8NLJC%S*RV%XXPSR39YS6XV6F:J$E98%TTPFBZ-4E2HU5VP5QKYB4YCJIBZ02C9A9L+QM79BUX5Z$VUV8N+UL%N6/Q4F1*:3BF2N9+:+/*XT-61P C9O+FI3*: 7CK7BTZ+D1UPF+SX-+//Y0-CRD%3485PHTC%6NO8/Y$Q+0QBV2+6O9F0:FJ1172/.VYFGB948I+6Y5+++D33FUYJO$$NBGM9JVT0JML$FYZPR482K0UM9%+PL.NI75K OWYE0GIAH%SSSCCLQ:F0PSW%4 .Z/BS85G4O.IZ3EXM-7 1 5LTL-F1F.H+9:A3$W470P5R3OEI$L9K *R.ZY.-Q::XCQZ8IXB%Q%ZY"
                }
            };
        }
    }

    public class PaperResponse
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("error")]
        public int Error { get; set; }

        [JsonPropertyName("qr")]
        public QrCode Qr { get; set; }
    }

    public class QrCode
    {
        [JsonPropertyName("data")]
        public string Data { get; set; }
    }
}
