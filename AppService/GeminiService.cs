using Mscc.GenerativeAI;

namespace Mini_Social_Media.AppService {
    public class GeminiService : IGeminiService {
        private readonly GenerativeModel _model;
        private readonly INotificationsRepository _notificationsRepository;
        public GeminiService(INotificationsRepository notificationRepository) {
            _notificationsRepository = notificationRepository;

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string apiKey = config["GeminiSettings:GeminiAPIKey"];
            string modelName = config["GeminiSettings:ModelName"];

            var api = new GoogleAI(apiKey);
            _model = api.GenerativeModel(model: modelName);
        }

        public async Task<bool> CheckPost(string caption) {
            string prompt = $@" You are a content moderation system following Facebook Community Standards.

                                Task:
                                Analyze the following caption and determine whether it VIOLATES community standards, including but not limited to:
                                - Profanity or vulgar language
                                - Hate speech or harassment
                                - Discrimination against individuals or groups
                                - Violence or threats
                                - Offensive or abusive content

                                RESPONSE RULES (STRICT):
                                - Respond with ONLY ONE WORD:
                                  - ""YES"" if the caption violates the standards
                                  - ""NO"" if it does not
                                - Do NOT explain
                                - Do NOT add any extra text, symbols, or new lines

                                Caption to review:
                                ""{caption}""
                                ";
            try {
                var result = await _model.GenerateContent(prompt);

                if (result == null || result.Text == null)
                    return true; 

                var content = result.Text.Trim().ToLower();

                Console.WriteLine($"Gemini Response: '{content}'");

                if (content.Contains("yes"))
                    return false;

                return true;
            }
            catch (Exception ex) {
                Console.WriteLine("Gemini Error: " + ex.Message);
                return true;
            }
        }
    }
}
