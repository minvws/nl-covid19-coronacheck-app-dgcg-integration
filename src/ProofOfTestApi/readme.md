# Proof of Test API

You can run the API in Visual Studio, or build it then run it with Docker (under ubuntu).

The development build included a swagger interface, this is very useful should you wish to integrate with the 
services ;) It's on the base url `/swagger`.

**TODO: automate the build in a build container**

## Testing the API

Currently there are two calls implemented: `GET proof/nonce` and `POST issue/proof`. These calls are both 
implemented in `ProofOfTestController.cs` with end-to-end style tests in the project `ProofOfTestApiTests`.

The JSON is rendered in camalCase, unless stated otherwise.

### GET proof/nonce

This endpoint accepts a GET and returns a JSON containing the nonce bytes encoded in a base64 string.

The curl looks like this:

    curl -X GET "https://localhost:5001/proof/nonce" -H  "accept: application/json"

The response looks like this:

    {
      "nonce": "pIdcGJ88P+EAAJeE1n+gng=="
    }

### GET issue/proof

This endpoint accepts a POST with a specific payload. It returns the proof as defined by the Go libraries (as json).

**QUESTION: should I accept the JSON directly instead of further encoding it as b64?** 


The POST payload looks like this:

    {
      "testType": "PCR",
      "nonce": "hgc3oMZzWd/rEcjdpHsNnw==",
      "commitments": "eyJuXzIiOiIzWjBFYWJYL1QrakZLN1Z5R3hXZmtRPT0iLCJjb21iaW5lZFByb29mcyI6W3siVSI6IldwS3FrQjlLZmtRUzRkV3E5OEJ3VWFYWHl6SEdqdUFiWDBFYkdUaFl5OFRVV3Y0QmZnQTlibXVlVE9zalZXaHRtcEJMamtCQnJyb3dhTGRhcUpHelY1emFQblIramdIdEtUOUEzSXdmWm5DQklZaXZ6c1ducWpib3gwdnJoYVJJTWE0RVNOVnBjc0NOeDJOL0djclh2czFUNjJra2xMTHhNM0svemlLSmlUZFRyZ3Q5dkNaM001TEp1WUlPNDQzVWlBTk1FV2oydldkVjMwSkl1R05XNVVLTDhVNm94SzBFK205KzhJeFAvUmJGclRKdUw3akd6RXFpcXNmQXJMdkFnUUp5V3NncEdIRlE1cGwzUnpRc2N4aDNQWnh0aThxbW55a2U5NG1iemF6YWhnYmJaUWltQWJTaXFlNTZrd3JnTzVVSzhPcmttZHNXcU5TelZGRzJ5QT09IiwiYyI6InBTaXUyVHJuemlONzdScUtQRlo2OHA5cEUvVjhMa1Uwa3ZGVFdPdFlCcGc9Iiwidl9wcmltZV9yZXNwb25zZSI6ImlnYVBqVVR6WDZXRDJKczkzNEQvWVgyQkIwbUp6K24rTStXdHJyQUx0VTVndUFIZFMyQmlUZGU1L0tEUlNrd0x2ZTdtY3FOR0l3ZzduYitZTEp4ZzFNS1k2OHExT0EzQ2FGbnZSb2JkUkJmQUlmeXMyOTBwOU53NXphRXVOMEZjSCtmNEJ4WHVaY3dYM0JDN1cwblJUTVBWZW8zMXZGanRlUGNCaTU0ZGlOS3I1QW5abUZTWVhFMHRMM2tnY1VacjFwcis1NG51RTBBc2h3b0xwbXZMQjhVUlhJMDZIQzJYM2svbjlYRG4zVXVKbERzU2FWV0g4N3FsdXhuMitIM09HT1dheHF3MWgyTThucytsRzZSaXQ0dmNHbXgxejM0TnE2amhKdXNLNDZ4aVNJaXJ6aStsNi9vcUt4UldIc05tQkQ5QXRaZjRPWVk3TC9kNDNWbjRRUHVBMHVYdjhHbU91dGdudXpmMXZsOWx0SjdkRDN2NnUxRlJlTGE1ZmJRNTRIS2dodjZMYlJ6MWcyMVdBQUdkd3R2cUpBdmpaYTZFdVpEeDBvNVZrZFk9Iiwic19yZXNwb25zZSI6ImtQS0xqVG1MQ1hBWVo0WG12Z1JROFVTK2YxU293enl3YW92TnU1YWJuYVJKQXo0eHMydHVLV1VkVmtabnloQ0hPZ2wxTFhSdkc2QXcvNmJ0ZUZoLzVNL0ZBNU5XUWp6ZGFqUT0ifV19"
    }

Where `testType` is currently defined as a free string, `nonce` is the base64 nonce received from `GET proof/nonce` and `commitments` is the result from the client crypto library encoded as a base64 string.

The curl looks like this:

    curl -X POST "https://localhost:5001/proof/issue" -H  "accept: application/json" -H  "Content-Type: application/json" -d "{\"testType\":\"PCR\",\"nonce\":\"hgc3oMZzWd/rEcjdpHsNnw==\",\"commitments\":\"eyJuXzIiOiIzWjBFYWJYL1QrakZLN1Z5R3hXZmtRPT0iLCJjb21iaW5lZFByb29mcyI6W3siVSI6IldwS3FrQjlLZmtRUzRkV3E5OEJ3VWFYWHl6SEdqdUFiWDBFYkdUaFl5OFRVV3Y0QmZnQTlibXVlVE9zalZXaHRtcEJMamtCQnJyb3dhTGRhcUpHelY1emFQblIramdIdEtUOUEzSXdmWm5DQklZaXZ6c1ducWpib3gwdnJoYVJJTWE0RVNOVnBjc0NOeDJOL0djclh2czFUNjJra2xMTHhNM0svemlLSmlUZFRyZ3Q5dkNaM001TEp1WUlPNDQzVWlBTk1FV2oydldkVjMwSkl1R05XNVVLTDhVNm94SzBFK205KzhJeFAvUmJGclRKdUw3akd6RXFpcXNmQXJMdkFnUUp5V3NncEdIRlE1cGwzUnpRc2N4aDNQWnh0aThxbW55a2U5NG1iemF6YWhnYmJaUWltQWJTaXFlNTZrd3JnTzVVSzhPcmttZHNXcU5TelZGRzJ5QT09IiwiYyI6InBTaXUyVHJuemlONzdScUtQRlo2OHA5cEUvVjhMa1Uwa3ZGVFdPdFlCcGc9Iiwidl9wcmltZV9yZXNwb25zZSI6ImlnYVBqVVR6WDZXRDJKczkzNEQvWVgyQkIwbUp6K24rTStXdHJyQUx0VTVndUFIZFMyQmlUZGU1L0tEUlNrd0x2ZTdtY3FOR0l3ZzduYitZTEp4ZzFNS1k2OHExT0EzQ2FGbnZSb2JkUkJmQUlmeXMyOTBwOU53NXphRXVOMEZjSCtmNEJ4WHVaY3dYM0JDN1cwblJUTVBWZW8zMXZGanRlUGNCaTU0ZGlOS3I1QW5abUZTWVhFMHRMM2tnY1VacjFwcis1NG51RTBBc2h3b0xwbXZMQjhVUlhJMDZIQzJYM2svbjlYRG4zVXVKbERzU2FWV0g4N3FsdXhuMitIM09HT1dheHF3MWgyTThucytsRzZSaXQ0dmNHbXgxejM0TnE2amhKdXNLNDZ4aVNJaXJ6aStsNi9vcUt4UldIc05tQkQ5QXRaZjRPWVk3TC9kNDNWbjRRUHVBMHVYdjhHbU91dGdudXpmMXZsOWx0SjdkRDN2NnUxRlJlTGE1ZmJRNTRIS2dodjZMYlJ6MWcyMVdBQUdkd3R2cUpBdmpaYTZFdVpEeDBvNVZrZFk9Iiwic19yZXNwb25zZSI6ImtQS0xqVG1MQ1hBWVo0WG12Z1JROFVTK2YxU293enl3YW92TnU1YWJuYVJKQXo0eHMydHVLV1VkVmtabnloQ0hPZ2wxTFhSdkc2QXcvNmJ0ZUZoLzVNL0ZBNU5XUWp6ZGFqUT0ifV19\"}"


The response looks like this:

    {
      "proof": {
        "c": "tHk+nswA/VSgQR41o+NlPEZUlBCdVbV7IK50/lrK0jo=",
        "e_response": "PfjLNp/UBFogQb88UQEArTQj4/mkg6zTFOg0UUGVsa9EQBCaZYG07AVgzrr7X5CterCGYcbV6DZEqCoP/UyknzL2fOeC5f1kqp/W69GIRqVFV2Cyjz6aITNQQBaiM4KkM21Cs2i32cmsPMC1GSW72ORpU0mPmP1RzWf0MuUdIQ=="
      },
      "signature": {
        "A": "ONKxjtJQUqMXolC0OltT2JWPua/7XqcFSuuCxNo25jh71C2S98JDYlSc2rkVC0G/RTNdY/gPfRWfzNOGIJvxSS3zRrnPBLFvG6Zo4rzIjsF+sQoIeUE/FNSAHTi7yART7MJIEbkHxn95Jw/dG8hTppbt1ALYpTXdKao6yFKRF0E=",
        "e": "EAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAa2ORygGdQClk2+FZuHl/",
        "v": "DJurgTXsDZgXHihHYpXwH81gmH+gan22XUPT07SiwuGdqNi1ikHDcXWSuf7Yae+nSIWh3fyIEoyIdNvloycrljVU7cClklrOLAsOyU45W07cjbBQATQmavoBsyZZaG/b/4aJFhfcuYHv6J72/8rm1UVqyk0i/0ROw/JukxbOFwkXm6FpfF2XUf3HvnSgEAbxPebxm5UKej7DxXx3fpHdELMKiyBICQjN0r6MwCU3PhbynISrjdbQsveeBh9id3O/kFISqMANSp6QmNPZ0jd4pOivOLFS",
        "KeyshareP": null
      }
    }
