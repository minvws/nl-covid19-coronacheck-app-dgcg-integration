// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool
{
    public class DgcgApp
    {
        private readonly IDgcgClient _dgcgClient;
        private readonly IDgcgClientConfig _dgcgClientConfig;
        private readonly Options _options;
        private readonly IJsonSerializer _serializer;
        private readonly TrustListValidator _validator;

        public DgcgApp(IDgcgClient dgcgClient, Options options, IJsonSerializer serializer, IDgcgClientConfig dgcgClientConfig,
                       TrustListValidator validator)
        {
            _dgcgClient = dgcgClient ?? throw new ArgumentNullException(nameof(dgcgClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _dgcgClientConfig = dgcgClientConfig ?? throw new ArgumentNullException(nameof(dgcgClientConfig));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task Run()
        {
            if (_options.Download)
            {
                Console.WriteLine($"Downloading the Trust List from: {_dgcgClientConfig.GatewayUrl}");

                var trustList = await _dgcgClient.GetTrustList();

                Console.WriteLine("Download successful!");
                Console.WriteLine();

                var trustListJson = _serializer.Serialize(trustList);

                Console.WriteLine("File:");
                Console.WriteLine(trustListJson);

                if (_options.Validate)
                {
                    Console.WriteLine();
                    Console.WriteLine("Validating the TrustList");
                    var errors = new List<string>();
                    var validDigitalGreenCertificates = _validator.Validate(trustList, errors);

                    if (errors.Any())
                    {
                        Console.WriteLine();
                        Console.WriteLine("Validation failed for a number of TrustList items:");
                        foreach (var error in errors)
                            Console.WriteLine(error);
                    }

                    Console.WriteLine();
                    var validDgcJson = _serializer.Serialize(validDigitalGreenCertificates);
                    Console.WriteLine("Valid DGCs:");
                    Console.WriteLine(validDgcJson);
                }

                if (!string.IsNullOrWhiteSpace(_options.Output))
                {
                    Console.WriteLine();
                    Console.WriteLine($"Writing output to: {_options.Output}");
                    try
                    {
                        // Write the raw output
                        File.WriteAllText(_options.Output, trustListJson);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("ERROR: unable to write to file. See error message below for more details.");
                        Console.WriteLine(e.Message);
                        Environment.Exit(1);
                    }
                }
            }
            else if (_options.Upload || _options.Revoke)
            {
                byte[] fileBytes = null;

                if (string.IsNullOrWhiteSpace(_options.File))
                {
                    Console.WriteLine("ERROR: no file provided!");

                    Environment.Exit(1);
                }

                try
                {
                    fileBytes = await File.ReadAllBytesAsync(_options.File);
                }
                catch (FileNotFoundException)
                {
                    HandleFileError();
                }
                catch (FileLoadException)
                {
                    HandleFileError();
                }
                catch (SecurityException)
                {
                    HandleFileError();
                }

                if (_options.Upload)
                {
                    Console.WriteLine($"Uploading the certificate {_options.File} to  {_dgcgClientConfig.GatewayUrl}");

                    var result = await _dgcgClient.Upload(fileBytes);

                    Console.WriteLine(result ? "Certificate successfully uploaded!" : "Certificate upload failed!");
                }
                else
                {
                    Console.WriteLine($"Revoking the certificate {_options.File} to  {_dgcgClientConfig.GatewayUrl}");

                    var result = await _dgcgClient.Revoke(fileBytes);

                    Console.WriteLine(result ? "Certificate successfully revoked!" : "Certificate revoke failed!");
                }
            }
            else
            {
                Console.WriteLine("No action selected.");
            }
        }

        private void HandleFileError()
        {
            Console.WriteLine($"ERROR: unable to open file: {_options.File}");

            Environment.Exit(1);
        }
    }
}
