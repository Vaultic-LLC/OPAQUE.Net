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

        public static T? TryExecute<T>(Func<T?> action, out Exception? ex)
        {
            ex = null;

            try
            {
                return action.Invoke();
            }
            catch (Exception e)
            {
                ex = e;
                Console.WriteLine(e);
            }

            return default;
        }
    }
}
