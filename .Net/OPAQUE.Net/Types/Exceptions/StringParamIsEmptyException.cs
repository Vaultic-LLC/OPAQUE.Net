namespace OPAQUE.Net.Types.Exceptions
{
    public class StringParamIsEmptyException : Exception
    {
        public StringParamIsEmptyException(string paramName) : base($"Parameter '{paramName}' can not be an empty string") { }

        public static void ThrowIfEmpty(string value, string paramName)
        {
            if (string.IsNullOrEmpty(value))
            {
                throw new StringParamIsEmptyException(paramName);
            }
        }
    }
}
