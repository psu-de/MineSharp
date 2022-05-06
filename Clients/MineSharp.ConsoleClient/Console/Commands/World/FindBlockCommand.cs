﻿using MineSharp.ConsoleClient.Client;
using MineSharp.ConsoleClient.Console.Commands.Arguments;
using MineSharp.Data.Blocks;
using PrettyPrompt.Highlighting;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MineSharp.ConsoleClient.Console.Commands.World {
    internal class FindBlockCommand : Command {

        EnumArgument<BlockType> BlockTypeArgument;

        public FindBlockCommand() {

            BlockTypeArgument = new EnumArgument<BlockType>("blockType");

            var desc = $"Searches for an Block of the specified [{BlockTypeArgument.Color}]Block Type[/]";

            this.Initialize("findBlock", desc, CColor.WorldCommand, BlockTypeArgument);
        }

        public override void DoAction(string[] argv, CancellationToken cancellation) {
            AnsiConsole.Status()
                .Start("Searching for [yellow]" + argv[0] + "[/]", async ctx => {
                    var blockType = BlockTypeArgument.GetValue(argv[0]);


                    var block = BotClient.Bot.FindBlockAsync(blockType, cancellation).GetAwaiter().GetResult();
                    if (cancellation.IsCancellationRequested) return;

                    if (block != null) {
                        AnsiConsole.MarkupLine("[green] Found Block: " + block + "[/]");
                    } else {
                        AnsiConsole.MarkupLine("[darkorange] No block was found![/]");
                    }

                });
        }
    }
}