// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Formatters;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Validator;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool
{
    public class DgcgApp
    {
        private readonly IDgcgClient _dgcgClient;
        private readonly IDgcgClientConfig _dgcgClientConfig;
        private readonly ITrustListFormatter _formatter;
        private readonly Options _options;
        private readonly TrustListValidator _validator;

        public DgcgApp(IDgcgClient dgcgClient, Options options, IDgcgClientConfig dgcgClientConfig, TrustListValidator validator, ITrustListFormatter formatter)
        {
            _dgcgClient = dgcgClient ?? throw new ArgumentNullException(nameof(dgcgClient));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _dgcgClientConfig = dgcgClientConfig ?? throw new ArgumentNullException(nameof(dgcgClientConfig));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
        }

        public async Task Run()
        {
            if (_options.Download)
                HandleDownload();
            else if (_options.Upload)
                HandleUpload();
            else if (_options.Upload || _options.Revoke)
                HandleRevoke();
            else
                Console.WriteLine("No action selected.");
        }

        private async void HandleDownload()
        {
            Console.WriteLine($"Downloading the Trust List from: {_dgcgClientConfig.GatewayUrl}");

            var trustList = await _dgcgClient.GetTrustList();

            Console.WriteLine("Download successful!" + trustList.Count);
            Console.WriteLine();
            Console.WriteLine("Validating the TrustList");
            var validatorResult = _validator.Validate(trustList);

            if (validatorResult.InvalidItems.Any())
            {
                Console.WriteLine();
                Console.WriteLine("Validation failed for a number of TrustList items:");
                Console.WriteLine();
                foreach (var item in validatorResult.InvalidItems)
                    Console.WriteLine($"ID: {item.Kid} [{item.Country}]: {validatorResult.GetReasonInvalid(item)}");
            }

            Console.WriteLine();
            Console.WriteLine("Valid DSCs:");
            Console.WriteLine();
            foreach (var item in validatorResult.ValidItems)
                Console.WriteLine($"ID: {item.Kid} [{item.Country}]");

            if (string.IsNullOrWhiteSpace(_options.Output)) return;

            Console.WriteLine();
            Console.WriteLine($"Writing output to: {_options.Output}");
            try
            {
                await File.WriteAllTextAsync(_options.Output, _formatter.Format(validatorResult.ValidItems, _options));
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: unable to write to file. See error message below for more details.");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
        }

        private async void HandleUpload()
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
            catch (Exception e) when (e is FileNotFoundException || e is FileLoadException || e is SecurityException)
            {
                HandleFileError();
            }

            Console.WriteLine($"Uploading the certificate {_options.File} to  {_dgcgClientConfig.GatewayUrl}");

            var result = await _dgcgClient.Upload(fileBytes);

            Console.WriteLine(result ? "Certificate successfully uploaded!" : "Certificate upload failed!");
        }

        private async void HandleRevoke()
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
            catch (Exception e) when (e is FileNotFoundException || e is FileLoadException || e is SecurityException)
            {
                HandleFileError();
            }

            Console.WriteLine($"Revoking the certificate {_options.File} to  {_dgcgClientConfig.GatewayUrl}");

            var result = await _dgcgClient.Revoke(fileBytes);

            Console.WriteLine(result ? "Certificate successfully revoked!" : "Certificate revoke failed!");
        }

        private void HandleFileError()
        {
            Console.WriteLine($"ERROR: unable to open file: {_options.File}");

            Environment.Exit(1);
        }
    }
}
