// // Copyright 2021 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// // Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// // SPDX-License-Identifier: EUPL-1.2

using System;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Services;

/// <summary>
///     Provides a JSON serializes which will not encode special character. This is comparable to the default serializers
///     from other JSON libraries and the defaults from other platforms.
/// </summary>
public class UnsafeJsonSerializer : IJsonSerializer
{
    private readonly JsonSerializerOptions _serializerOptions;

    public UnsafeJsonSerializer()
    {
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
    }

    public string Serialize<TContent>(TContent input)
    {
        return input == null ? string.Empty : JsonSerializer.Serialize(input, _serializerOptions);
    }

    public TContent Deserialize<TContent>(string input)
    {
        return JsonSerializer.Deserialize<TContent>(input, _serializerOptions) ?? throw new InvalidOperationException();
    }
}
