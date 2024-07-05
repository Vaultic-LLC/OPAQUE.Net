namespace OPAQUE.Net.Base.Helpers
{
    public static class FunctionHelper
    {
        public static T? TryExecute<T>(Func<T?> action)
        {
            try
            {
                return action.Invoke();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return default;
        }
    }
}
