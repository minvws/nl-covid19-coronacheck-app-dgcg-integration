using System;

namespace NL.Rijksoverheid.CoronaTester.BackEnd.Common.Config
{
    [Serializable]
    public class MissingConfigurationValueException : Exception
    {
        public MissingConfigurationValueException(string name) : base($"Missing value: {name}.")
        {
        }
    }
}