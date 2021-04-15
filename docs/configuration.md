# Index


* Overview
* Configuration of the APIs
	- Proof of Test API and Static Proof API
	- Issuer API
	- Shared Configuration
	- Logging
* Configuration of dependencies
	- Creating a CMS certificate
	- Installing Redis


# Overview

This solution contains three API projects which can all be deployed seperatly:

* ProofOfTestApi: Middleware, handles sessions and provides an API for the apps to talk to.
* StaticProofApi: Middleware provides an API for the creation of static (i.e. fixed, one time) proofs.
* IssuerApi: Contains the crypto services, this is an internal microservice called by ProofOfTestApi

You can run the projects locally, one at a time, using the Docker images provided in each project.

In order to run the projects you will need to configure them. 

# Configuration of the APIs

The configuration is stored in the standard .Net Core appsettings.json format. For local development you can make an `appsettings.development.json` file in the project and put your settings there. Some of the project use a database so this step is required.


## Proof of Test API and Static Proof API

### Redis Session Store

The Session Store is responsible for storing the session state (currently the Nonce) between the GetNonce() and CreateProof() calls. It should be distributed and persistant. The data in the session store will only be persisted for a few seconds in the 95% case.

	"RedisSessionStore": {
		"Configuration": "localhost:6379"
	}

* Configuration: provide a valid Redis configuration string, the format of the configuration string is documentated here: https://stackexchange.github.io/StackExchange.Redis/Configuration.html

### Redis Test Result Log

The Session Store is responsible for storing the a hash of the identifier of the test results. It should be distributed, persistant and support a transactional approach. This store is checked for every issuance request to ensure that we only issue one Proof of Test for each test result. The data in this store will be persisted for a number of days; to be decided by the local laws and privacy policy.
 
	"RedisTestResultLog": {
		"Configuration": "localhost:6379",
		"Duration": 72,
		"Salt": "abcdefg"
	}

* Configuration: provide a valid Redis configuration string, the format of the configuration string is documentated here: https://stackexchange.github.io/StackExchange.Redis/Configuration.html
* Duration: number of hours that the hash will be stored. Positive 32bit integer.
* Salt: salt used for the hash function. This can be any valid JSON string. **This is security sensative data**.

### Issuer API client

This configures the endpoint where the IssuerAPI service is located.
  
  "IssuerApi": {
    "BaseUrl": "https://localhost:44332"
  },
  
* BaseUrl: Base url for the IssuerAPI in your environment. Must be a valid URI with no trailing '/'.

### Test signing

This configures the Test Provider certificates. The certificates contain the public key and are in the `pem` format.
 
  "TestSigning": {
    "ProviderCertificates": {
      "TST001": "..\\..\\..\\..\\..\\test\\test-certificates\\TST001.pem",
      "TST002": "..\\..\\..\\..\\..\\test\\test-certificates\\TST002.pem"
    }
  }
  
* ProviderCertificates: This contains a string-keyed dictionary which links the test provider code to the path of their certificate.
* ProviderCertificates[n].Key: Test Provider Code, a unique string.
* ProviderCertificates[n].Value: UNC path to the x.509 certificate for the given test provider containing only the public key, packaged in the `pem` format. For ACC/PROD this must be a PKi Overheid certificate, see our security document for details.


## Issuer API

### Issuer Certificates

The issuer certificates configure the CL certificates which will be used by the CL library.

	"IssuerCertificates": {
		"UseEmbedded": false,
		"PathPublicKey": "Resources/public_key.xml",
		"PathPrivateKey": "Resources/private_key.xml"
	}

For details on generating the keys please see the documentation of the CL library.

* UseEmbedded: For ACC/PROD deployments this is always `false`. Set to `true` to use the development certificates embedded into the assembly.
* PathPublicKey: UNC path to the public key.
* PathPrivateKey: UNC path to the private key.
  
### Issuer

This section contains configuration for the CL Issuer library.

  "Issuer": {
    "PublicKeyIdentifier": "testPk"
  },
  
* PublicKeyIdentifier: name of the Public Key used by the CL issuer library.

## Partial Disclosure List

The partial disclosure list is a CSV file which contains the list used by the Partial Disclosure service to provide privacy-preserving attribute sets. The contents of the file are documented in our architecture document.

	"PartialDisclosureList": {
		"Path": "..\\..\\..\\..\\..\\test\\configuration\\partial-disclosure-list.csv"
	}

* Path: UNC path to the partial-disclosure-list.


## Shared

These configuration options are shared by all currently APIs.
  
### CMS Signing Certificates  

The CMS signing certificates are used to sign all of the responses made by our service.

  "Certificates": {
    "CmsSigning": {
      "UseEmbedded": false,
      "Path": "..\\..\\..\\..\\Common\\Resources\\sign.p12",
      "Password": "123456"
    }
  }

* UseEmbedded: For ACC/PROD deployments this is always `false`. Set to `true` to use the development certificates embedded into the assembly.
* Path: UNC path to the x.509 CMS signing certificate. For ACC/PROD this must be a PKi Overheid certificate, see our security document for details. For testing/development you can generate your own certificate, see the guide in this document.
* Password:  Password for the private key of the above signing certificate **This is security sensative data**.

### API Signing

This configuration allows you to enable or disable the signing of the API responses.

When it is enabled all of the endpoints will encode the normal JSON response as base64, sign it with our CMS certificate, and put the results in the wrapper:

	{
		"payload": "b64string",
		"signature": "b64string"
	}

The configuration:

	"ApiSigning": {
		"WrapAndSignResult": false
	}
  
* WrapAndSignResult: `true` to enable the signed responses, `false` to disable.


## Logging

Logging configuration is found in NLog.config; please refer to thei documentation from NLog for details on how to configure this.

https://nlog-project.org/config/


## Creating a CMS Certificate

As mentioned earlier, for the response singing, you'll need an x.509 certificate. One is included in the package for testing, or you can follow the guide below to generate a new one.

Prereqs:

* OpenSSL
* Bash or bash-compatible terminal (Windows users: WSL 2.0 or Conemu/Cmder)

0. Save this in a file called ext.cnf:

    ```
    [ signing_key ]
    nsComment = For testing only and no this is not the real thing. Duh.
    keyUsage = nonRepudiation, digitalSignature, keyEncipherment
    subjectKeyIdentifier=hash
    authorityKeyIdentifier=keyid:always,issuer
    basicConstraints = CA:FALSE
    ```

1. Create an x509 certificate

    ```
    openssl req -new -keyout sign.key -nodes -subj "/C=NL/O=Test/OU=CoronaCheck/CN=Signing cert" | openssl x509 -extfile ext.cnf --extensions signing_key -req -signkey sign.key -out sign.pub
    ```

2. Create a pkcs12 from the x509 keypair with the password '123456'

    ```
    openssl pkcs12 -export -out=sign.pfx -in sign.pub -inkey sign.key -nodes -passout pass:123456
    ```

3. PROFIT

## Installing Redis

We use Redis under Linux; for Windows users there is a port, however it's very easy to install it under `Windows Subsystem for Linux 2` so we recommend using that. You can of course use Docker or a VM. If you choose a Debian-based distribution (Debian, Ubuntu, Linux Mint etc) then you can follow this guide. This small guide has been written using WSL2 with an Ubuntu image.

First, use `apt-get` (or whatever tooling your distro provides) to install redis:
 
```
sudo apt-get update
sudo apt-get upgrade
sudo apt-get install redis-server
```

Check that it installed OK:

```
redis-cli -v
```

Then you can control the service with these commands:

```
sudo service redis-server start
sudo service redis-server stop
sudo service redis-server restart
```

..and interact with Redis via the CLI:

```
redis-cli
```

Redis will now be running on `localhost:6379`.