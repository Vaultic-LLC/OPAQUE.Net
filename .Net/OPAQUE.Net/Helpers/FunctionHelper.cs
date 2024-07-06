namespace OPAQUE.Net.Helpers
{
    public static class FunctionHelper
    {
        public static T? TryExecute<T>(Func<T?> action)
        {
            try
            {
                return action.Invoke();
            }
            catch { /* Exception occured in rust. */ }

            return default;
        }
    }
}
