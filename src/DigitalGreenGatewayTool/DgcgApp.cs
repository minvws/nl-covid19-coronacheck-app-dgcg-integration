// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System;
using System.IO;
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

        public DgcgApp(IDgcgClient dgcgClient, Options options, IJsonSerializer serializer, IDgcgClientConfig dgcgClientConfig)
        {
            _dgcgClient = dgcgClient ?? throw new ArgumentNullException(nameof(dgcgClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _serializer = serializer ?? throw new ArgumentNullException(nameof(serializer));
            _dgcgClientConfig = dgcgClientConfig ?? throw new ArgumentNullException(nameof(dgcgClientConfig));
        }

        public async Task Run()
        {
            if (_options.Download)
            {
                Console.WriteLine($"Downloading the Trust List from: {_dgcgClientConfig.GatewayUrl}");

                var trustList = await _dgcgClient.GetTrustList();

                Console.WriteLine("Download successful!");
                Console.WriteLine();
                Console.WriteLine(_serializer.Serialize(trustList));
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

                    await _dgcgClient.Upload(fileBytes);

                    Console.WriteLine("Certificate successfully uploaded!");
                }
                else
                {
                    Console.WriteLine($"Revoking the certificate {_options.File} to  {_dgcgClientConfig.GatewayUrl}");

                    await _dgcgClient.Revoke(fileBytes);

                    Console.WriteLine("Certificate successfully revoked!");
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
