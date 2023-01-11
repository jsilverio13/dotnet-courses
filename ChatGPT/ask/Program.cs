
using System.Text;
using Newtonsoft.Json;

namespace ask
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            if (args.Length > 0)
            {
                var client = CreateClient();

                var responseString = await RequestChatGpt(args, client);

                GuessAndPrintText(responseString);
            }
            else
            {
                Console.WriteLine(" ---> You need to provide some input");
            }

        }

        private static void GuessAndPrintText(string responseString)
        {
            try
            {
                var dyData = JsonConvert.DeserializeObject<dynamic>(responseString);
                string text = dyData!.choices[0].text;
                string guess = GuessCommand(text);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($" ---> My guess at the command prompt is: {guess}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine($" ---> Could not deserialize the JSON: {ex.Message}");
            }
        }

        private static async Task<string> RequestChatGpt(string[] args, HttpClient client)
        {
            string json = "{\"model\": \"text-davinci-001\", \"prompt\": \"" + args[0]  + "\", \"temperature\": 1, \"max_tokens\": 100 }";

            var content = new StringContent(json, Encoding.UTF8, Constants.Text.ApplicationJson);

            var response = await client.PostAsync(Constants.Request.Url, content);

            string responseString = await response.Content.ReadAsStringAsync();

            return responseString;
        }

        private static HttpClient CreateClient()
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add(nameof(Constants.Request.Authorization), string.Concat(Constants.Request.Authorization.Bearer, " ", Constants.Request.Authorization.ApiKey));

            return client;
        }

        private static string GuessCommand(string raw)
        {
            Console.WriteLine(" ---> GPT-3 API Returned Text:");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(raw);

            var lastIndex = raw.LastIndexOf('\n');

            string guess = raw.Substring(lastIndex + 1);

            Console.ResetColor();

			TextCopy.ClipboardService.SetText(guess);

            return guess;
        }
    }

}
