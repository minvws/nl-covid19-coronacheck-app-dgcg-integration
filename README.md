# COVID-19 <Project Name> - <platform>

## Introduction
This repository contains the <platform> implementation of the Dutch COVID-19 <project or app name>.

* The <platform> app is located in the repository you are currently viewing.
* The <related> can be found here: <related repo>

## Local development setup

### Prereqs

* Dotnet 5 SDK (https://dotnet.microsoft.com/download/dotnet/5.0)
* Go issuer C library built for your platform (see [Issuer library](#issuer-library))  
* Redis is required for the ProofOfTestApi, for a small guide to installing it see [Installing Redis](#installing-redis))  
* Postgres 13+ is required to run `VerifierAppApi` and `HolderAppApi`

### Nice to have

* Visual Studio 2019 with ReSharper
* Rider (latest)
* Windows users: a good terminal - cmder (https://cmder.net/) or WSL2 (https://docs.microsoft.com/en-us/windows/wsl/install-win10)
* Docker

### Prereq for linux/osx

We use two scripts to build and publish our code; by default these are not executable, so you need to make them executable.

Make the build script executable:

```sudo chmod +x src/build.sh ```

Make the publish script executable:

```sudo chmod +x src/build.sh ```
	
### Buidling and publishing the solution

We provide a `build.bat` (`build.sh`) and `publish.bat` (`publish.sh`) which build the entire solution, and publish the API respectivly. Both scripts are in the `src` directory.

Before you run the scripts, you need to replace the Go library - `src/IssuerInterop/issuer.dll` and `src/IssuerInterop/issuer.h` - with your platform-specific build.

### Running the services

This solution contains three API projects which can all be deployed seperatly:

* ProofOfTestApi: Middleware, handles sessions and provides an API for the app to talk to.
* StaticProofApi: Middleware provides an API for external b2b systems to talk to.
* IssuerApi: Contains the crypto services, this is an internal microservice called by ProofOfTestApi
* ProofOfTestApi: Contains the crypto services called by Holder app.
* HolderAppApi: Contains the non-crypto services called by the Holder app.
* VerifierAppApi: Contains the service called by the Verifier app.

The services in HolderAppApi and VerifierAppApi are designed to be used behind a CDN, the services they contain produce results which do not change very often.

You can run the projects locally, one at a time, using the Docker images provided in each project.

In order to run the projects you will need to configure them. 

## Issuer library

The backend relies on the CTCL issuer library for the crypto operations. We will include a small guide on building this libary here.

The libary is available in the CTCL repository: https://github.com/minvws/nl-covid19-coronatester-ctcl-core

### Prereqs

Install the version of these tools appropriate to your platform/cpu.

* Go 1.15.7+ installed and in your path.
  * Check that it's installed by running: `go version`
* GCC 9.2.0+ installed and in your path.
  * Check that it's installed by running: `gcc --version`

### Building the library

* Ensure that you've checked out `nl-covid19-coronatester-ctcl-core`.
* Move to the directory `issuer/cinterface`.
* Build the libray: `go build -buildmode=c-shared -o issuer.dll`

You can then copy `issuer.dll` and `issuer.h` from `nl-covid19-coronatester-ctcl-core/issuer/cinterface` to the folder `src/IssuerInterop` in `nl-covid19-coronatester-app-backend-private`.

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

## Configuration of the services

The configuration is stored in the standard .Net Core appsettings.json format. For local development you can make an `appsettings.development.json` file in the project and put your settings there. Some of the project use a database so this step is required.

### Configuration: database

Both the `VerifierAppApi` and `HolderAppApi` use a Postgres database. Create a database for this project, run the scripts in `db/schema` to create the schema, run the scripts in `db/data` to add the data then update the connection string in the configuration the APIs can access the database:

```
  "ConnectionStrings": {
    "tester": "host=your_database_server_host_name;database=your_datbaase_name;user id=your_user_id;password=your_password;"
  },
```

### Configuration: Issuer keys

The issuer uses two certificates - a public and provide key. We currently ship two methods to load these files - from the filesystem or embedded in the assembly. For development you can use the test certificates which are embedded into the assembly using this configuration:

```
	"IssuerCertificates": {
		"UseEmbedded": true
	}
```

This is highly recommended because the path is not always predictable, different editors and deployments can all.. differ. We want to support the broadest range of development environments and using the embedded resources makes that easier.

When running the system on a proper webserver you need to provide your own keys and configure them like this:

```
	"IssuerCertificates": {
		"UseEmbedded": false,
		"PathPublicKey": "Resources/public_key.xml",
		"PathPrivateKey": "Resources/private_key.xml"
	}
```

### Configuration: CMS call signing 

The services can be configured to wrap all of the service call responses with a signature. You can enable this by first enabling the wrapper:

```
  "ApiSigning": {
    "WrapAndSignResult": true
  }
```

You will also need to configure the CMS certificate for each of the services; use the following configuration to load the cert from the file system:
  
```
  "Certificates": {
    "CmsSigning": {
      "Path": "path/to/cert",
      "Password": "password",
      "UseEmbedded": false
    }
  }
```

We currently build the test CMS certificate into the assembly, if you want to use that (recommended for local development for reasons mentioned earlier) then you can use this configuration:

```
  "Certificates": {
    "CmsSigning": {
      "Path": "sign.p12",
      "Password": "123456",
      "UseEmbedded": true
    }
  }
```

### Configuration: redis

TODO

## Creating a CMS Certificate

As mentioned earlier, for the response singing, you'll need an x.509 certificate. One is included in the package for testing, or you can follow the guide below to generate a new one.

Prereqs:

* OpenSSL
* Bash or bash-compatible terminal (Windows users: WSL 2.0 or Conemu/Cmder)

0. Save this in a file called ext.cnf:

    ```
    [ tester_signing_key ]
    nsComment = For testing only and no this is not the real thing. Duh.
    keyUsage = nonRepudiation, digitalSignature, keyEncipherment
    subjectKeyIdentifier=hash
    authorityKeyIdentifier=keyid:always,issuer
    basicConstraints = CA:FALSE
    ```

1. Create an x509 certificate

    ```
    openssl req -new -keyout sign.key -nodes -subj "/C=NL/O=Test/OU=CoronaCheck/CN=Signing cert" | openssl x509 -extfile ext.cnf --extensions tester_signing_key -req -signkey sign.key -out sign.pub
    ```

2. Create a pkcs12 from the x509 keypair with the password '123456'

    ```
    openssl pkcs12 -export -out=sign.pfx -in sign.pub -inkey sign.key -nodes -passout pass:123456
    ```

3. PROFIT

## Development & Contribution process

The development team works on the repository in a private fork (for reasons of compliance with existing processes) and shares its work as often as possible.

If you plan to make non-trivial changes, we recommend to open an issue beforehand where we can discuss your planned changes.
This increases the chance that we might be able to use your contribution (or it avoids doing work if there are reasons why we wouldn't be able to use it).

Note that all commits should be signed using a gpg key.
