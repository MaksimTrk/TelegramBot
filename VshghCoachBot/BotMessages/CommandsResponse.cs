using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot;

namespace VshghCoachBot.BotMessages
{
    internal class CommandsResponse
    {
        public static string help = "To get the exercise - /exercises\r\nTo set your parameters(height and weight) - /set [height] in cm [weight] in kg" +
            "\r\nTo get body mass index - /bmi\r\nTo delete your parameters -\n/deleteparameters\r\nTo set ypur SBD personal records - /sbdset [squat] [bench press] [deadlift] in kg\r\nTo get your current SBD - /sbd\r\nTo delete your SBD - /deletesbd";
        public static string exercises = "Choose a muscle group:\r\n/back\r\n/chest\r\n/shoulders\r\n/arms\r\n/legs\r\n/neck\r\n/waist\r\nOr cardio exersices:\r\n/cardio\r\n\r\n";
        public static string start = "Let`s start!\r\nAbove all, my main function is to provide you with a random exercise for your desired muscle group. " +
            "\r\nAlso you can set your height and waight, this is necessary so that you can get your body mass index."+
            "\r\nAnd if you are experienced athlete, you can save and update your current personal records in squat, bench press and deadlift (use kg)."+
            "\r\n\r\nFor more information about commands - \n/help";
    }
}
