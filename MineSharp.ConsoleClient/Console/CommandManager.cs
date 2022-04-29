﻿using MineSharp.ConsoleClient.Console.Commands;
using MineSharp.ConsoleClient.Console.Commands.Chat;
using MineSharp.ConsoleClient.Console.Commands.Entity;
using MineSharp.ConsoleClient.Console.Commands.Misc;
using MineSharp.ConsoleClient.Console.Commands.Player;
using MineSharp.ConsoleClient.Console.Commands.Prompt;
using MineSharp.ConsoleClient.Console.Commands.World;
using PrettyPrompt.Highlighting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.ConsoleClient.Console {
    internal static class CommandManager {

        public static Dictionary<string, Command> Commands = new Dictionary<string, Command>();

        static CommandManager() {
            CommandPalette.Initialize();
        }

        public static void RegisterCommand(Command command) {
            Commands.Add(command.Name, command);
        }

        public static bool TryGetCommand(string name, out Command command) {
            if (!Commands.ContainsKey(name)) {
                command = null;
                return false;
            }
            command = Commands[name];

            return true;
        }

        private static class CommandPalette {

            static bool _initialized = false;

            public static void Initialize() {

                if (_initialized) return;

                _initialized = true;

                //Prompt
                RegisterCommand(new HelpCommand());
                RegisterCommand(new ShowCommand());

                //World
                RegisterCommand(new FindBlockCommand());
                RegisterCommand(new FindBlocksCommand());
                RegisterCommand(new GetBlockAtCommand());
                RegisterCommand(new MineBlockAtCommand());

                //Chat
                RegisterCommand(new SayCommand());

                //Player
                RegisterCommand(new RespawnCommand());
                RegisterCommand(new AttackCommand());
                RegisterCommand(new SetRotationCommand());
                RegisterCommand(new LookAtCommand());

                //Entities
                RegisterCommand(new GetEntitiesCommand());

                //Misc
                RegisterCommand(new DisplayCommand());
            }

        }
    }
}
