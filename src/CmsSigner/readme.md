# CMS Signer tool

This is a small tool, it takes a file as input along with two certificates (your signing cert and a cert containing the full chain). It then signs the file and packs it in the format defined by our APIs. Then, finally, it outputs that to std-out.

## Usage

The tool is simple and has a tiny fixed interface.

    CmsSigner file_to_sign path-to-x509-CMS-certificate password path-to-x509-CMS-chain

e.g.

    CmsSigner bananas.json sign.p12 123456 mychain.p7b

