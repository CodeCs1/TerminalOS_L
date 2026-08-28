namespace TerminalOS_Lgen3
{
    public record Result<R,E> {

        private readonly R? result_ctx;
        private readonly E? error_ctx;

        public Result(R? result=default, E? error=default) {
            result_ctx = result;
            error_ctx = error;
        }

        public static Result<R, E> Ok(R result) => new(result);
        public static Result<R, E> Error(E error) => new(error: error);

        public R Unwrap()
        {
            if (result_ctx != null) return result_ctx;
            throw new NullReferenceException("Null Result");
        }
        public R Expect(string msg)
        {
            if (result_ctx != null) return result_ctx;
            throw new NullReferenceException($"{msg}: {error_ctx}");
        }
        public R Unwrap_or(R default_value) => result_ctx ?? default_value;
        public E Unwrap_err_or(E default_value) => error_ctx ?? default_value;
    }
}
