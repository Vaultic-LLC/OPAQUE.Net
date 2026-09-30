namespace OPAQUE.Net.Helpers
{
    public static class FunctionHelper
    {
        public static T? TryExecute<T>(Func<T?> action) => action.Invoke();

        public static T? TryExecute<T>(Func<T?> action, out Exception? e)
        {
            e = null;
            return action.Invoke();
        }
    }
}
