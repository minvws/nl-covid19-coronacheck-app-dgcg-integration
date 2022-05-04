// // Copyright 2022 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System;
using System.IO;
using System.Security;
using System.Threading.Tasks;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Commands
{
    public class UploadCommand : ICommand
    {
        private readonly IDgcgClient _dgcgClient;
        private readonly IDgcgClientConfig _dgcgClientConfig;
        private readonly UploadOptions _options;

        public UploadCommand(IDgcgClient dgcgClient, IDgcgClientConfig dgcgClientConfig, UploadOptions options)
        {
            _dgcgClient = dgcgClient ?? throw new ArgumentNullException(nameof(dgcgClient));
            _dgcgClientConfig = dgcgClientConfig ?? throw new ArgumentNullException(nameof(dgcgClientConfig));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task Execute()
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
            catch (Exception e) when (e is FileNotFoundException or FileLoadException or SecurityException)
            {
                Console.WriteLine($"ERROR: unable to open file: {_options.File}");

                Environment.Exit(1);
            }

            Console.WriteLine($"Uploading the certificate {_options.File} to  {_dgcgClientConfig.GatewayUrl}");

            var result = await _dgcgClient.Upload(fileBytes);

            Console.WriteLine(result ? "Certificate successfully uploaded!" : "Certificate upload failed!");
        }
    }
}
