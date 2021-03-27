// Copyright 2020 De Staat der Nederlanden, Ministerie van Volksgezondheid, Welzijn en Sport.
// Licensed under the EUROPEAN UNION PUBLIC LICENCE v. 1.2
// SPDX-License-Identifier: EUPL-1.2
// using System;

using System.IO;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Files
{
    public class FileSystemFileLoader : IFileLoader
    {
        public byte[] Load(string path)
        {
            return File.ReadAllBytes(path);
        }
    }
}
