using System;
using System.Collections.Generic;
using Telegram.Bot.Args;
using AntiAntiSwearingBot.Commands;

namespace AntiAntiSwearingBot
{
    public interface IChatCommand
    {
        string Execute(CommandString cmd, MessageEventArgs messageEventArgs);
    }

    public class ChatCommandRouter
    {
        string Username { get; }
        Dictionary<string, IChatCommand> Commands { get; }

        public ChatCommandRouter(string username)
        {
            Username = username;
            Commands = new Dictionary<string, IChatCommand>();
        }

        public string Execute(object sender, MessageEventArgs args)
        {
            var text = args.Message.Text;
            if (CommandString.TryParse(text, out var cmd))
            {
                if (cmd.UserName != null && cmd.UserName != Username)
                    return null;
                if (Commands.ContainsKey(cmd.Command))
                    return Commands[cmd.Command].Execute(cmd, args);
            }
            return null;
        }

        public void Add(IChatCommand c, params string[] cmds)
        {
            foreach (var cmd in cmds)
            {
                if (Commands.ContainsKey(cmd))
                    throw new ArgumentException($"collision for {cmd}, commands {Commands[cmd].GetType()} and {c.GetType()}");
                Commands[cmd] = c;
            }
        }
    }
}
