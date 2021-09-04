using Discord;
using Discord.Net;
using Discord.WebSocket;
using Discord.Commands;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace questhelperhelper.Modules
{
    //For Commands to be available, and have the context passed to them, we must inherit ModuleBase
    public class QuestStatusCommands : ModuleBase
    {
        [Command("quest")]
        public async Task HelloCommand()
        {
            //Initialize empty string builder for reply
            var sb = new StringBuilder();
            
            //Get user info from the Context
            var user = Context.User;

            //Build out the reply
            sb.AppendLine($"You are -> [{user.Username}]");
            sb.AppendLine("I must now say, World!");

            //Send Simple string reply
            await ReplyAsync(sb.ToString());
        }
    }
}