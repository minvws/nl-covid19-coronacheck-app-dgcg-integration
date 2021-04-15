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

You can run the projects locally, one at a time, using the Docker images provided in each project.

In order to run the projects you will need to configure them. 

## Issuer library

The backend relies on the CTCL issuer library for the crypto operations. We will include a small guide on building this libary here.

The libary is available in the CTCL repository: https://github.com/minvws/nl-covid19-coronacheck-cl-core

### Prereqs

Install the version of these tools appropriate to your platform/cpu.

* Go 1.15.7+ installed and in your path.
  * Check that it's installed by running: `go version`
* GCC 9.2.0+ installed and in your path.
  * Check that it's installed by running: `gcc --version`

### Building the library

* Ensure that you've checked out `nl-covid19-coronacheck-cl-core`.
* Move to the directory `issuer/cinterface`.
* Build the libray: `go build -buildmode=c-shared -o issuer.dll`

You can then copy `issuer.dll` and `issuer.h` from `nl-covid19-coronacheck-cl-core/issuer/cinterface` to the folder `src/IssuerInterop` in `nl-covid19-coronacheck-app-backend`.

### Configuration

You can find information over how to configure the services in docs/configuration.md.


## Development & Contribution process

The development team works on the repository in a private fork (for reasons of compliance with existing processes) and shares its work as often as possible.

If you plan to make non-trivial changes, we recommend to open an issue beforehand where we can discuss your planned changes.
This increases the chance that we might be able to use your contribution (or it avoids doing work if there are reasons why we wouldn't be able to use it).

Note that all commits should be signed using a gpg key.
