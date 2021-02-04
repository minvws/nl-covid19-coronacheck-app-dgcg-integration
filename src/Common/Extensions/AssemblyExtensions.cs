using System;
using System.IO;
using System.Reflection;
using System.Text;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Extensions
{
    public static class AssemblyExtensions
    {
        /// <summary>
        /// Extracts an embedded UTF-8 resource file from the given assembly
        /// </summary>
        /// <param name="assembly">Assembly containing the embedded file</param>
        /// <param name="resourcePath">Path to resource relative to the root (i.e. excluding assembly name)</param>
        /// <returns>File contents in an UTF-8 string</returns>
        public static string GetEmbeddedResourceAsString(this Assembly assembly, string resourcePath)
        {
            using var stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{resourcePath}");
            if (stream == null)
            {
                throw new Exception("Unable to load key");
            }
            using var reader = new StreamReader(stream, Encoding.UTF8);
            return reader.ReadToEnd();
        }

        public static Stream? GetEmbeddedResourceAsStream(this Assembly assembly, string resourcePath)
        {
            return assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{resourcePath}");
        }
    }
}
