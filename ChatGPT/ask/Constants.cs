using System.Net.Http.Headers;

namespace ask
{
    public static class Constants
    {
        public static class Request
        {
            public const string Url = "https://api.openai.com/v1/completions";
            public static class Authorization
            {
                public const string Bearer = "Bearer";
                public const string ApiKey = "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";
            }
        }
        public static class Text
        {

            public const string StringReplace = "[[REPLACE]]";

            public const string ApplicationJson = "application/json";
        }
    }
}