namespace IOptionTest.Options
{
    public class ExceptionOptions
    {
        public enum ExceptionHandlerEnum
        {
            UseExceptionHandler,
            UsePages,
            UseMyMiddleWare,
            UseHellang,
            DotNet7
        }
        public static ExceptionHandlerEnum ExceptionHandler { get; set; }
    }
}
