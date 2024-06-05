using System;
using System.Runtime;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using VshghCoachBot;
using VshghCoachBot.BotMessages;
using Npgsql;

namespace APIprot.Clients
{
    public class TelegramClient
    {
        private readonly Client _client;
        Database db = new Database();

        public TelegramClient(Client client)
        {
            _client = client;

        }
        public async Task Start(string token)
        {
            var botClient = new TelegramBotClient(token);
            botClient.StartReceiving(Update, Error);
            await Task.Delay(Timeout.Infinite);
        }

        private async Task Update(ITelegramBotClient client, Update update, CancellationToken token)
        {
            var message = update.Message;
            if (message.Text != null)
            {
                string bodyPart = "";
                var botMessage = "";


                switch (message.Text.ToLower())
                {
                    case "/start":
                        botMessage = CommandsResponse.start;
                        break;
                    case "/help":
                        botMessage = CommandsResponse.help;
                        break;
                    case "/exercises":
                        botMessage = CommandsResponse.exercises;
                        break;
                    case string text when text.Contains("/set"):

                        string[] userParams = text.Split(' ');
                        if (userParams.Length != 3)
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "To set or update your parameters - \n/set [height] [weight]");
                            return;
                        }

                        if (!double.TryParse(userParams[1], out double userHeight) || !double.TryParse(userParams[2], out double userWeight))
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Incorrect input format. Height and Weight must be numbers.");
                            return;
                        }
                        long UserId = message.Chat.Id;
                        await db.InsertUserParameters(UserId, userHeight, userWeight);
                        botMessage = "Your parameters has been saved.";
                        break;
                    case "/bmi":
                        long userId = message.Chat.Id;
                        var userParameters = await db.GetUserParameters(userId);
                        if (userParameters == null)
                        {
                            botMessage = "No parameters found. Please set your height and weight using the /set command.";
                        }
                        else
                        {
                            double height = userParameters.Value.height;
                            double weight = userParameters.Value.weight;

                            double bmi = weight / ((height / 100) * (height / 100));
                            string bmiCategory;

                            if (bmi < 18.5)
                            {
                                bmiCategory = "Underweight";
                            }
                            else if (bmi >= 18.5 && bmi < 25)
                            {
                                bmiCategory = "Normal weight";
                            }
                            else if (bmi >= 25 && bmi < 30)
                            {
                                bmiCategory = "Overweight";
                            }
                            else
                            {
                                bmiCategory = "Obese";
                            }
                            botMessage = $"{bmiCategory}\r\nYour body mass index - {Convert.ToString(bmi)}";
                        }
                        break;
                    case "/deleteparameters":
                        db.DeleteUserParameters(message.Chat.Id);
                        botMessage = "Your parameters has been deleted.";
                        break;
                    case string text when text.Contains("/sbdset"):
                        string[] parameterssbd = text.Split(' ');
                        if (parameterssbd.Length != 4)
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "To set or update your SBD prs - /sbdset [squat] [bench] [deadlift]");
                            return;
                        }

                        double squat, bench, deadlift;
                        if (!double.TryParse(parameterssbd[1], out squat) || !double.TryParse(parameterssbd[2], out bench) || !double.TryParse(parameterssbd[3], out deadlift))
                        {
                            await client.SendTextMessageAsync(message.Chat.Id, "Incorrect input format. Height and weight must be numbers.");
                            return;
                        }                    
                        await db.InsertUserSBD(message.Chat.Id, squat, bench, deadlift);
                        botMessage = $"Your SBD has been saved.";
                        break;
                    case "/sbd":
                        long chatId = message.Chat.Id;
                        var sbdData = await db.GetUserSBD(chatId);
                        if (sbdData == null)
                        {
                            botMessage = "No SBD data found.";
                        }
                        else
                        {
                            double squatValue = sbdData.Value.squat;
                            double benchValue = sbdData.Value.bench;
                            double deadliftValue = sbdData.Value.deadlift;
                            double total = squatValue + benchValue + deadliftValue;
                            botMessage = $"Squat: {squatValue} kg\nBench Press: {benchValue} kg\nDeadlift: {deadliftValue} kg\nTotal: {total} kg";
                        }
                        break;
                    case "/deletesbd":
                        db.DeleteUserSBD(message.Chat.Id);
                        botMessage = "Your SBD has been deleted.";
                        break;
                    case "/back":
                        bodyPart = "back";
                        break;
                    case "/chest":
                        bodyPart = "chest";
                        break;
                    case "/shoulders":
                        bodyPart = "shoulders";
                        break;
                    case "/arms":
                        bodyPart = "upper arms";
                        break;
                    case "/legs":
                        bodyPart = "upper legs";
                        break;
                    case "/cardio":
                        bodyPart = "cardio";
                        break;
                    case "/neck":
                        bodyPart = "neck";
                        break;
                    case "/waist":
                        bodyPart = "waist";
                        break;

                }
                if (!string.IsNullOrEmpty(bodyPart))
                {
                    var exercises = await _client.GetExercises(bodyPart);
                    var random = new Random();
                    var randomExercise = exercises[random.Next(exercises.Count)];

                    var response = $"{randomExercise.Name}\nEquipment: {randomExercise.Equipment}\n" +
                        $"Target muscle: {randomExercise.Target}\nSecondary muscles:\n";
                    foreach (var muscle in randomExercise.SecondaryMuscles)
                    {
                        response += $"{muscle}\n";
                    }
                    response += "How to do:\n";
                    foreach (var instruction in randomExercise.Instructions)
                    {
                        response += $"{instruction}\n";
                    }
                    response += $"{randomExercise.GifUrl}";

                    await client.SendTextMessageAsync(message.Chat.Id, response);
                }
                else if (!string.IsNullOrEmpty(botMessage))
                {
                    var response = botMessage;
                    await client.SendTextMessageAsync(message.Chat.Id, response);
                }
                else
                {
                    var errorMessage = "Sorry, I couldn't understand. Please try again.";
                    await client.SendTextMessageAsync(message.Chat.Id, errorMessage);
                }
            }
        }
        private async Task Error(ITelegramBotClient client, Exception exception, CancellationToken token)
        {
            Console.WriteLine($"Error: {exception.Message}");
            await Task.CompletedTask;
        }
    }
}

