# COVID-19 Digital Green Certificate Gateway Integration

## Introduction

This repository contains the implementation of the Dutch COVID-19 Digital Green Certificate Gateway integration

* The <platform> app is located in the repository you are currently viewing.
* The <related> can be found here: <related repo>

## Local development setup

### Prereqs

* Dotnet 5 SDK (https://dotnet.microsoft.com/download/dotnet/5.0)

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

We provide a `build.bat` (`build.sh`) and `publish.bat` (`publish.sh`) which build and publish the solution respectively.

### Running the tool

See the `readme.md` in `src/DigitalGreenGatewayTool` for more info!

## Development & Contribution process

The development team works on the repository in a private fork (for reasons of compliance with existing processes) and shares its work as often as possible.

If you plan to make non-trivial changes, we recommend to open an issue beforehand where we can discuss your planned changes.
This increases the chance that we might be able to use your contribution (or it avoids doing work if there are reasons why we wouldn't be able to use it).

Note that all commits should be signed using a gpg key.
