using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using QHH.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHH.Modules
{
    [Name("FAQs")]
    public class FAQModule : QHHModuleBase
    {
        public FAQModule(IServiceProvider serviceProvider, IConfiguration configuration)
            : base(serviceProvider, configuration)
        {

        }

        [Command("faqs", RunMode = RunMode.Async)]
        public async Task FAQs()
        {
            var faqs = await this.DataAccessLayer.GetFAQs();

            if (faqs.Count() == 0)
            {
                var noFAQs = new QHHEmbedBuilder()
                    .WithTitle("No FAQs found")
                    .WithDescription("This server does not have any FAQs yet.")
                    .WithStyle(EmbedStyle.Error)
                    .Build();

                await this.Context.Channel.SendMessageAsync(embed: noFAQs);
                return;
            }

            string description = string.Join(", ", faqs.Select(x => x.Name));
            //TODO: Refactor into DataAccessLayer
            //var prefix = await this.DataAccessLayer.GetPrefixAsync();
            var prefix = "!";

            var list = new QHHEmbedBuilder()
                .WithTitle($"FAQs ({faqs.Count()})")
                .WithDescription(description)
                .WithFooter($"Use \"{prefix}faq name\" to view an FAQ")
                .WithStyle(EmbedStyle.Information)
                .Build();
            await this.Context.Channel.SendMessageAsync(embed: list);
        }

        [Command("faq", RunMode = RunMode.Async)]
        [Alias("f")]
        public async Task FAQ([Remainder] string argument)
        {
            var arguments = argument.Split(" ");

            if (arguments.Count() == 1 && arguments[0] != "create" && arguments[0] != "edit" && arguments[0] != "transfer" && arguments[0] != "delete")
            {
                var faq = await this.DataAccessLayer.GetFAQ(arguments[0]);
                if (faq == null)
                {
                    var embed = new QHHEmbedBuilder()
                        .WithTitle("Not Found")
                        .WithDescription("The FAQ you requested could not be found.")
                        .WithStyle(EmbedStyle.Error)
                        .Build();

                    await this.Context.Channel.SendMessageAsync(embed: embed);
                    return;
                }

                var faqEmbed = new QHHEmbedBuilder()
                    .WithTitle(faq.Name)
                    .WithDescription(faq.Content)
                    .WithStyle(EmbedStyle.Information)
                    .Build();

                await this.Context.Channel.SendMessageAsync(embed: faqEmbed);
                return;
            }

            var socketGuildUser = this.Context.User as SocketGuildUser;

            switch (arguments[0])
            {
                case "create":
                    var faq = await this.DataAccessLayer.GetFAQ(arguments[1]);
                    if (faq != null)
                    {
                        var embed = new QHHEmbedBuilder()
                            .WithTitle("Already Exists")
                            .WithDescription("There already exists an FAQ with that name.")
                            .WithStyle(EmbedStyle.Error)
                            .Build();

                        await this.Context.Channel.SendMessageAsync(embed: embed);
                        return;
                    }

                    if (!this.Context.User.IsPromoted())
                    {
                        var embed = new QHHEmbedBuilder()
                            .WithTitle("Access Denied!")
                            .WithDescription("You need to be a contributor or administrator to create an FAQ.")
                            .WithStyle(EmbedStyle.Error)
                            .Build();

                        await this.Context.Channel.SendMessageAsync(embed: embed);
                        return;
                    }

                    await this.DataAccessLayer.CreateFaq(arguments[1], this.Context.User.Id, string.Join(" ", arguments.Skip(2)));

                    //TODO: Refactor into DataAccessLayer
                    //var prefix = await this.DataAccessLayer.GetPrefixAsync();
                    var prefix = "!";

                    var created = new QHHEmbedBuilder()
                        .WithTitle("FAQ Created!")
                        .WithDescription($"The FAQ has been successfully created. You can view it by using `{prefix}faq {arguments[1]}`")
                        .WithStyle(EmbedStyle.Success)
                        .Build();
                    await this.Context.Channel.SendMessageAsync(embed: created);
                    break;
                case "edit":
                    var foundFAQ = await this.DataAccessLayer.GetFAQ(arguments[1]);
                    if (foundFAQ == null)
                    {
                        var embed = new QHHEmbedBuilder()
                            .WithTitle("Not Found!")
                            .WithDescription("That tag could not be found.")
                            .WithStyle(EmbedStyle.Error)
                            .Build();

                        await this.Context.Channel.SendMessageAsync(embed: embed);
                        return;
                    }

                    if (foundFAQ.OwnerId != this.Context.User.Id && !socketGuildUser.GuildPermissions.Administrator)
                    {
                        var embed = new QHHEmbedBuilder()
                            .WithTitle("Access Denied!")
                            .WithDescription("You need to be the owner of this tag or an administrator to edit the content of this FAQ.")
                            .WithStyle(EmbedStyle.Error)
                            .Build();

                        await this.Context.Channel.SendMessageAsync(embed: embed);
                        return;
                    }

                    await this.DataAccessLayer.EditFAQContent(arguments[1], string.Join(" ", arguments.Skip(2)));

                    var edited = new QHHEmbedBuilder()
                        .WithTitle("FAQ Content Modified")
                        .WithDescription("The content of the tag was successfully modified.")
                        .WithStyle(EmbedStyle.Success)
                        .Build();
                    await this.Context.Channel.SendMessageAsync(embed: edited);
                    break;

                case "transfer":
                    var faqToTransfer = await this.DataAccessLayer.GetFAQ(arguments[1]);
                    if (faqToTransfer == null)
                    {
                        var embed = new QHHEmbedBuilder()
                            .WithTitle("Not Found!")
                            .WithDescription("that FAQ could not be found.")
                            .WithStyle(EmbedStyle.Error)
                            .Build();

                        await this.Context.Channel.SendMessageAsync(embed: embed);
                        return;
                    }

                    if (!MentionUtils.TryParseUser(arguments[2], out ulong userId) || this.Context.Guild.GetUser(userId) == null)
                    {
                        var embed = new QHHEmbedBuilder()
                            .WithTitle("Invalid User")
                            .WithDescription("Please provide a valid user.")
                            .WithStyle(EmbedStyle.Error)
                            .Build();

                        await this.Context.Channel.SendMessageAsync(embed: embed);
                        return;
                    }

                    if (faqToTransfer.OwnerId != this.Context.User.Id && !socketGuildUser.GuildPermissions.Administrator)
                    {
                        var embed = new QHHEmbedBuilder()
                            .WithTitle("Access Denied!")
                            .WithDescription("You need to be the owner of this tag or an administrator to edit the content of this FAQ.")
                            .WithStyle(EmbedStyle.Error)
                            .Build();

                        await this.Context.Channel.SendMessageAsync(embed: embed);
                        return;
                    }

                    await this.DataAccessLayer.EditFAQOwner(arguments[1], userId);
                    
                    var success = new QHHEmbedBuilder()
                        .WithTitle("FAQ Ownership Transferred")
                        .WithDescription($"The ownership of the tag has been transferred to <@{userId}>.")
                        .WithStyle(EmbedStyle.Success)
                        .Build();

                    await this.Context.Channel.SendMessageAsync(embed: success);
                    break;

                case "delete":

                    var faqToDelete = await this.DataAccessLayer.GetFAQ(arguments[1]);

                    if (faqToDelete == null)
                    {
                        var embed = new QHHEmbedBuilder()
                            .WithTitle("Not Found!")
                            .WithDescription("that FAQ could not be found.")
                            .WithStyle(EmbedStyle.Error)
                            .Build();

                        await this.Context.Channel.SendMessageAsync(embed: embed);
                        return;
                    }

                    if (faqToDelete.OwnerId != this.Context.User.Id && !socketGuildUser.GuildPermissions.Administrator)
                    {
                        var embed = new QHHEmbedBuilder()
                            .WithTitle("Access Denied!")
                            .WithDescription("You need to be the owner of this tag or an administrator to edit the content of this FAQ.")
                            .WithStyle(EmbedStyle.Error)
                            .Build();

                        await this.Context.Channel.SendMessageAsync(embed: embed);
                        return;
                    }

                    await this.DataAccessLayer.DeleteFAQ(arguments[1]);

                    var deleted = new QHHEmbedBuilder()
                        .WithTitle("FAQ Deleted!")
                        .WithDescription("The FAQ was successfully delted.")
                        .WithStyle(EmbedStyle.Success)
                        .Build();

                    await this.Context.Channel.SendMessageAsync(embed: deleted);
                    break;
            }
        }
    }
}
