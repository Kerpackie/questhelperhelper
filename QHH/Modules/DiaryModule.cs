using Discord;
using Discord.Commands;
using Discord.WebSocket;
using QHH.Common;
using QHH.Data;
using QHH.Data.Context;
using QHH.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QHH.Modules
{
    public class DiaryModule : ModuleBase
    {
        //private readonly QHHDbContext context;
        private readonly AchievementDiaries achievementDiaries;

        public DiaryModule(AchievementDiaries achievementDiaries)
        {
            //this.context = context;
            this.achievementDiaries = achievementDiaries;
        }

        [Command("diaries", RunMode = RunMode.Async)]
        public async Task Diaries()
        {
            await Context.Channel.TriggerTypingAsync();

            var diaries = await this.achievementDiaries.GetAchievementDiaries();
            if (diaries != null)
            {
                var diaryListEmbedBuilder = new DiaryEmbedBuilder()
                    .WithDescription("No ETA's are provided on Achievement Diaries - work is continuing as fast as we can!");
                foreach (var diary in diaries)
                {
                    var diaryEmbed = new EmbedFieldBuilder()
                        .WithName($"{diary.DiaryName}")
                        .WithValue($"{diary.Status}")
                        .WithIsInline(true);

                    diaryListEmbedBuilder.AddField(diaryEmbed);

                    switch (diary.Status)
                    {
                        case DevelopmentStatus.NoProgress:
                            {
                                diaryEmbed.WithValue("<:diary:881209561330122832> Not Started! :red_circle:");
                                break;
                            }

                        case DevelopmentStatus.InDevelopment:
                            {
                                diaryEmbed.WithValue("<:diary:881209561330122832> In Development! :yellow_circle:");
                                break;
                            }

                        case DevelopmentStatus.PRSubmitted:
                            {
                                diaryEmbed.WithValue("<:diary:881209561330122832> PR Submitted! :purple_circle:");
                                break;
                            }

                        case DevelopmentStatus.Live:
                            {
                                diaryEmbed.WithValue("<:diary:881209561330122832> Live! :green_circle:");
                                break;
                            }
                    }
                }

                await Context.Channel.SendMessageAsync("", false, diaryListEmbedBuilder.Build()); 
            }
            else
            {
                var embedError = new ErrorEmbedBuilder()
                    .WithDescription("There are no Achievement Diaries found. Please add one or report this Error to Kerpackie.")
                    .Build();
                await Context.Channel.SendMessageAsync(embed: embedError);
            }
        }

        [Command("adddiary", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task AddDiary([Remainder] string name)
        {
            var reactions = new List<Emoji>();
            reactions.Add(new Emoji("\uD83D\uDD34"));
            reactions.Add(new Emoji("\uD83D\uDFE1"));
            reactions.Add(new Emoji("\uD83D\uDFE3"));
            reactions.Add(new Emoji("\uD83D\uDFE2"));

            await Context.Channel.TriggerTypingAsync();
            var diaries = await this.achievementDiaries.GetAchievementDiaries();

            if (diaries.Any(x => x.DiaryName == name))
            {
                var embedError = new ErrorEmbedBuilder()
                    .WithDescription("That Achievement Diary already exists!")
                    .Build();
                await Context.Channel.SendMessageAsync(embed: embedError);
                return;
            }

            var description = new StringBuilder()
                .AppendLine()
                .AppendLine($"Use this panel to control the status of the {name} Achievement Diary.")
                .AppendLine()
                .AppendLine("You can use the emotes below to adjust the status of the Diary. The comment will not update with the current status, so you may need to run the !diaries command to ensure that it updated successfully. ");

            var embed = new AddDiaryEmbedBuilder()
                .WithTitle($"{name} Diary Status Control Panel")
                .WithDescription(description.ToString())
                .AddField("Diary Added By:", $"{Context.User.Username}")
                .WithCurrentTimestamp()
                .Build();

            var message = await Context.Channel.SendMessageAsync(embed: embed);

            await message.AddReactionsAsync(reactions.ToArray());

            await this.achievementDiaries.CreateAchievementDiary(Context.User.Id, message.Id, name);
            //await ReplyAsync($"The diary {name} has been added to the DB by {Context.User}!");
        }

        [Command("deldiary", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task DelDiary([Remainder] string name)
        {
            await Context.Channel.TriggerTypingAsync();
            var diaries = await this.achievementDiaries.GetAchievementDiaries();

            if (diaries.Any(x => x.DiaryName == name))
            {
                var embedSuccess = new AddDiaryEmbedBuilder()
                    .WithDescription($"Successfully deleted the Achievement Diary: `{name}`")
                    .Build();

                await this.achievementDiaries.DeleteAchievementDiary(name);
                await Context.Channel.SendMessageAsync(embed: embedSuccess);
                return;
            }

            var embedError = new ErrorEmbedBuilder()
                .WithDescription($"Diary: `{name}` not deleted! Is it a valid diary or have you spelt it correctly?")
                .Build();
            await Context.Channel.SendMessageAsync(embed: embedError);
        }

        [Command("editdiaryname", RunMode = RunMode.Async)]
        [RequireUserPermission(GuildPermission.Administrator)]
        public async Task EditDiaryName(string name = null, [Remainder] string args = null)
        {
            if (name == null && args == null)
            {
                var embed = new ErrorEmbedBuilder()
                    .WithDescription("Please pass through a valid argument, in the format DiaryName, NewDiaryName.")
                    .Build();

                await Context.Channel.SendMessageAsync(embed: embed);
                return;
            }

            if (name != null && args == null)
            {
                var embed = new ErrorEmbedBuilder()
                    .WithDescription("Please pass through a new name for the Achievement Diary.")
                    .Build();

                await Context.Channel.SendMessageAsync(embed: embed);
                return;
            }
            else
            {
                await this.achievementDiaries.ModifyAchievementDiaryName(name, args);
                var embedSuccess = new DiaryEmbedBuilder()
                    .WithDescription($"Diary: `{name}` has been renamed to `{args}`")
                    .Build();
                await Context.Channel.SendMessageAsync(embed: embedSuccess);
            }
        }
    }
}
