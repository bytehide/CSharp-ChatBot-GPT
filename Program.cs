using Newtonsoft.Json;
using RestSharp;

namespace ChatGPTChatbot
{
    public class ChatGPTClient
    {
        private readonly string _apiKey;
        private readonly RestClient _client;

        // Constructor that takes the API key as a parameter
        public ChatGPTClient(string apiKey)
        {
            _apiKey = apiKey;
            // Initialize the RestClient with the ChatGPT API endpoint
            _client = new RestClient("https://api.openai.com/v1/engines/text-davinci-003/completions");
        }

        // Method to send a message to the ChatGPT API and return the response
        public string SendMessage(string message)
        {
            // Check for empty input
            if (string.IsNullOrWhiteSpace(message))
            {
                return "Sorry, I didn't receive any input. Please try again!";
            }

            try
            {
                // Create a new POST request
                var request = new RestRequest("", Method.Post);
                // Set the Content-Type header
                request.AddHeader("Content-Type", "application/json");
                // Set the Authorization header with the API key
                request.AddHeader("Authorization", $"Bearer {_apiKey}");

                // Create the request body with the message and other parameters
                var requestBody = new
                {
                    prompt = message,
                    max_tokens = 100,
                    n = 1,
                    stop = (string?)null,
                    temperature = 0.7,
                };

                // Add the JSON body to the request
                request.AddJsonBody(JsonConvert.SerializeObject(requestBody));

                // Execute the request and receive the response
                var response = _client.Execute(request);

                // Deserialize the response JSON content
                var jsonResponse = JsonConvert.DeserializeObject<dynamic>(response.Content ?? string.Empty);

                // Extract and return the chatbot's response text
                return jsonResponse?.choices[0]?.text?.ToString()?.Trim() ?? string.Empty;
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the API request
                Console.WriteLine($"Error: {ex.Message}");
                return "Sorry, there was an error processing your request. Please try again later.";
            }
        }

        class Program
        {
            static void Main(string[] args)
            {
                // Replace with your ChatGPT API key
                string apiKey = "your_api_key_here";
                // Create a ChatGPTClient instance with the API key
                var chatGPTClient = new ChatGPTClient(apiKey);

                // Display a welcome message
                Console.WriteLine("Welcome to the ChatGPT chatbot! Type 'exit' to quit.");

                // Enter a loop to take user input and display chatbot responses
                while (true)
                {
                    // Prompt the user for input
                    Console.ForegroundColor = ConsoleColor.Green; // Set text color to green
                    Console.Write("You: ");
                    Console.ResetColor(); // Reset text color to default
                    string input = Console.ReadLine() ?? string.Empty;

                    // Exit the loop if the user types "exit"
                    if (input.ToLower() == "exit")
                        break;

                    // Send the user's input to the ChatGPT API and receive a response
                    string response = chatGPTClient.SendMessage(input);

                    // Display the chatbot's response
                    Console.ForegroundColor = ConsoleColor.Blue; // Set text color to blue
                    Console.Write("Chatbot: ");
                    Console.ResetColor(); // Reset text color to default
                    Console.WriteLine(response);
                }
            }
        }
    }
}