using System;

namespace NL.Rijksoverheid.CoronaCheck.BackEnd.Common.Config
{
    [Serializable]
    public class MissingConfigurationValueException : Exception
    {
        public MissingConfigurationValueException(string name) : base($"Missing value: {name}.")
        {
        }
    }
}
