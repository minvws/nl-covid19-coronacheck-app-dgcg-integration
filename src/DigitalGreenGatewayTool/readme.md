This project implements our interop with DGCG.

The gateway is here:

https://github.com/eu-digital-green-certificates/dgc-gateway

The documentation there is now pretty good.. because we improved it :-)


# How to use this tool

Get help:

```dggt --help```

Download the certs:

```dggt -d -o path/to/save/trustlist.json```

Upload a cert:

```dggt -u -f path/to/cert.```

* File must be a DER encoded certificate; see the example below if you need to convert from PEM

Revoke a cert:

```dggt -r -f path/to/cert.```

* File must be a DER encoded certificate; see the example below if you need to convert from PEM


# Command-line interface

```-p | --pause```

Pauses once the task has executed until a key press.

## TrustList

```-d | --download```

Download the trust list.

```-c <country--two-letter-iso-code> | --country <country--two-letter-iso-code>```

Only download trust list for the given country. `<country--two-letter-iso-code>` must be a valid two-letter ISO country code.

```-t <type> | --type <type>```

Only download trust list items of the given type. NOTE: this can result in validation failure as certain certificates are validated by those of other types.

`<type>` can be any of: `AUTHENTICATION`, `UPLOAD`, `CSCA`, `DSC`.

```-o <path-to-file> | --output <path-to-file> ```

Output file; the TrustList will be output here.

```--unformatted```

Outputs the raw unformatted trust list if set. Otherwise the output will be the output expected of our apps.

```-v | --validate```

Validates the certificates on the trust list. This validates the trust chain as well as the format. Errors will be printed to the console and will not be included in the output.

## Upload

```-u | --upload```

Upload the given file.

```-f <file> | --file <file>```

Required! Provides the file (containing the DSC encoded as a DER) to be uploaded.

`<file>` is a valid path/file name.


## Revoke

```-r | --revoke```

Revokes the given file.

```-f <file> | --file <file>```

Required! Provides the file (containing the DSC encoded as a DER) to be uploaded.

`<file>` is a valid path/file name.


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


# Converting the DGCG certs to PFX

If you're using a local install of the gateway then you'll need to convert the certificates into PFX so that they can be used here.

```
openssl pkcs12 -inkey key_ta.pem -in cert_ta.pem -export -out ta.pfx
```