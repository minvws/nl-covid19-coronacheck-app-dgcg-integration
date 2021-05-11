This project implements our interop with DGCG.

The gateway is here:

https://github.com/eu-digital-green-certificates/dgc-gateway

The documentation there is now pretty good.. because we improved it :-)

# How to use this tool

Download the certs:

```dggt -d```

Upload a cert:

```dggt -u -f path/to/cert.```

* File must be a DER encoded certificate; see the example below if you need to convert from PEM

Revoke a cert:

```dggt -r -f path/to/cert.```

* File must be a DER encoded certificate; see the example below if you need to convert from PEM

# Configuration

The configuration looks like this:

```
{
  "Certificates": {
    "Authentication": {
      "UseEmbedded": false,
      "Path": "auth.pfx",
      "Password": ""
    },
    "Signing": {
      "UseEmbedded": false,
      "Path": "sign.pfx",
      "Password": ""
    },
    "SigningChain": {
      "UseEmbedded": false,
      "Path": "only_used_if_IncludeChainInSignature_is_TRUE.pfx",
      "Password": ""
    }
  },
  "DgcgClient": {
    "SendAuthenticationHeaders": true,
    "GatewayUrl": "http://localhost:8080",
    "IncludeChainInSignature": false,
    "IncludeCertsInSignature": true 
  } 
}
```

You need to configure the path to the authentication certificate and the password.

Under `DgcgClient`, the `GatewayUrl` must be set; for test this is: https://test-dgcg-ws.tech.ec.europa.eu

The flag `SendAuthenticationHeaders` can be set in order to include the TLS authentication headers; this is
probably always required. For EFGS this was optional in some circumstances.

The flag `IncludeChainInSignature` when set will include the cert chain in the signature. You must also 
configure the 'SigningChain' certificate if this is set.

The flag `IncludeCertsInSignature` when set will include the certificates in the signature.

# Creating some DGC signing certs

You'll need these to test stuff. The CSCA is created when following the DGC readme.

Generate the DGC certs like this:

```
openssl req -new -keyout key_dgc1.pem -nodes -out key_dgc1.csr
```

Then convert the certificate into DER format:

```
 openssl x509 -outform der -in cert_dgc2.pem -out dgc2.der
 ```
