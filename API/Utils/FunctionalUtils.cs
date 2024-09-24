namespace API.Utils.FunctionalUtils {
    public static partial class Util {
        public static Func<TResult> Bind<T, TResult>(this Func<T, TResult> func, T arg) {
            return () => func(arg);
        }

        public static Func<T2, TResult> Bind<T1, T2, TResult>(this Func<T1, T2, TResult> func, T1 arg) {
            return t2 => func(arg, t2);
        }
    }
}
