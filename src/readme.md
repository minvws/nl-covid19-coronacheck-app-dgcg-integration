# Building

First install:

* Golang compiler
* GCC
* .Net Core / .Net 5

Then build the go assembly by executing this in ```src/IssuerInterop```:

```go build -buildmode=c-shared -o issuer.dll```

Then tests can be ran in Visual Studio or Code. TODO: dotnet cli.

