// Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Client;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Formatters;
using NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Validator;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.DigitalGreenGatewayTool.Commands
{
    public class DownloadCommand : ICommand
    {
        private readonly IDgcgClient _dgcgClient;
        private readonly IDgcgClientConfig _dgcgClientConfig;
        private readonly ITrustListFormatter _formatter;
        private readonly DownloadOptions _options;
        private readonly TrustListValidator _validator;

        public DownloadCommand(IDgcgClient dgcgClient, IDgcgClientConfig dgcgClientConfig, TrustListValidator validator, ITrustListFormatter formatter,
                               DownloadOptions options)
        {
            _dgcgClient = dgcgClient ?? throw new ArgumentNullException(nameof(dgcgClient));
            _dgcgClientConfig = dgcgClientConfig ?? throw new ArgumentNullException(nameof(dgcgClientConfig));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
            _formatter = formatter ?? throw new ArgumentNullException(nameof(formatter));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task Execute()
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
                await File.WriteAllTextAsync(_options.Output, _formatter.Format(validatorResult.ValidItems, _options.ThirdPartyKeysFile));
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: unable to write to file. See error message below for more details.");
                Console.WriteLine(e.Message);
                Environment.Exit(1);
            }
        }
    }
}
