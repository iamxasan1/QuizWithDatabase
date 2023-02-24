using QuizWithDatabase.services;
using QuizWithDatabase.models;
using JFA.Telegram.Console;
using Telegram.Bot;
using Telegram.Bot.Types;



var botManager = new TelegramBotManager();
var bot = botManager.Create("5931456022:AAGrnYI-S-i6Wx8uRgaiNlPV9fjfWeDhGrY");
var Services = new Services();

var botDetails = await bot.GetMeAsync();
Console.WriteLine(botDetails.FirstName + " quiz test bot is started succesfully");

botManager.Start(NewMessage);

void NewMessage(Update update)
{
    if (update.Type != Telegram.Bot.Types.Enums.UpdateType.Message) return;
    var chatId = update.Message!.From!.Id;
    var message = update?.Message?.Text;
    var user = Services.UserService.GetUser(chatId);
    
    switch (user.UserNextStep)
    {
        case ENextStep.Start: Start(user);
            break;
        case ENextStep.ChooseMenu:ChooseMenu(user, message);
            break;
        case ENextStep.SignUp: SignUp(user, message);
            break;
        case ENextStep.SignIn: SignIn(user, message);
            break;
        case ENextStep.SignUpPassword:SignUpPassword(user, message);
            break;
        case ENextStep.SignInPassword:SignInPassword(user, message);
            break;
        default:
            break;
    }


    Console.WriteLine(message);
}

void Start(UserX user)
{
    bot.SendTextMessageAsync(user.ChatId, SendMainMenu());
    user.UserNextStep = ENextStep.ChooseMenu;
}

string SendMainMenu()
{
    return "main menu:\nSignUp\nSignIn"; 
}

void ChooseMenu(UserX user, string message)
{
    if(message == "SignUp")
    {
        user.UserNextStep = ENextStep.SignUp;
        bot.SendTextMessageAsync(user.ChatId, "please enter your login");
    }
    else if(message == "SignIn")
    {
        user.UserNextStep= ENextStep.SignIn;
        bot.SendTextMessageAsync(user.ChatId, "please enter your login");

    }
    else
    {
        bot.SendTextMessageAsync(user.ChatId, "please eenter an available menu");
        user.UserNextStep = ENextStep.ChooseMenu;
    }
}

void SignUp(UserX user, string? message)
{
    user.Login = message;
    bot.SendTextMessageAsync(user.ChatId, "please enter your password");
    user.UserNextStep = ENextStep.SignUpPassword;
}
void SignUpPassword(UserX user, string? message)
{
    user.Password = message;
    if (Services.UserService.CheckuserLogged(user.ChatId))
    {
        Services.UserService.Users.Add(user);
        bot.SendTextMessageAsync(user.ChatId, "you`ve signed up succesfully");
    }
    else
    {
        bot.SendTextMessageAsync(user.ChatId, "something get wrong");
    }
    bot.SendTextMessageAsync(user.ChatId, SendMainMenu());
    Services.UserService.Save();
    user.UserNextStep = ENextStep.Start;
}


void SignIn(UserX user, string? message)
{
    user.Login = message;
    bot.SendTextMessageAsync(user.ChatId, "please enter your password");
    user.UserNextStep = ENextStep.SignInPassword;
}
void SignInPassword(UserX user, string? message)
{
    user.Password = message;
    var newUser = Services.UserService.Users.FirstOrDefault(u => u.Login == user.Login && u.Password == message);
    if (newUser is not null)
    {
        bot.SendTextMessageAsync(user.ChatId, $"congratulations you signed in {user.Login} account succesfully");
        user.UserNextStep= ENextStep.QuizMenu;
    }
    else
    {
        bot.SendTextMessageAsync(user.ChatId, "sorry there is no user with this password and login try again");
        user.UserNextStep = ENextStep.Start;
    }
}





