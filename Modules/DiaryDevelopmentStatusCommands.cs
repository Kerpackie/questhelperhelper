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
using questhelperhelper.Database;
using questhelperhelper.Services;
using Microsoft.Extensions.DependencyInjection;

namespace questhelperhelper.Modules
{
    // for commands to be available, and have the Context passed to them, we must inherit ModuleBase
    public class DiaryDevelopmentStatusCommands : ModuleBase
    {
        private readonly QHHEntities _db;
        private List<String> _validStatus = new List<String>();
        private List<String> _validDiary = new List<String>();
        private readonly IConfiguration _config;

        public DiaryDevelopmentStatusCommands(IServiceProvider services)
        {
            // we can pass in the db context via depedency injection
            _db = services.GetRequiredService<QHHEntities>();
            _config = services.GetRequiredService<IConfiguration>();

            _validStatus.Add("live");
            _validStatus.Add("merged");
            _validStatus.Add("indev");
            _validStatus.Add("notstarted");
        }

        [Command("diarylist")]
        [Alias("diarieslist", "diaries", "ad", "diary")]
        public async Task ListDiarys()
        {            
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();

            var user = Context.User;

            var diariesFull = await _db.DiaryDevelopmentStatus.ToListAsync();
            if (diariesFull != null)
            {
                sb.AppendLine("There is NO ETA on diaries - work is continuing as fast as we can!");

                foreach (var diary in diariesFull)
                {
                    var diaryListEmbed = new EmbedFieldBuilder()
                        .WithName($"{diary.DiaryName}")
                        .WithValue($"{diary.DiaryStatus}")
                        .WithIsInline(true);

                    embed.AddField(diaryListEmbed);

                    switch (diary.DiaryStatus)
                    {
                        case "live":
                        {
                            diaryListEmbed.WithValue("<:diary:881209561330122832> Live! :green_circle:");
                            break;
                        }
                        case "merged":
                        {
                            diaryListEmbed.WithValue("<:diary:881209561330122832> Merged! :purple_circle:");
                            break;
                        }
                        case "indev":
                        {
                            diaryListEmbed.WithValue("<:diary:881209561330122832> In Development! :yellow_circle:");
                            break;
                        }        
                        case "notstarted":
                        {
                            diaryListEmbed.WithValue("<:diary:881209561330122832> Not Started! :red_circle:");
                            break;
                        }
                    }             
                }
            }
            else
            {
                sb.AppendLine("Oh, there are no Achievement Diary Helpers found! Please report this to Kerpackie#6211");
            }

            // set embed
            embed.Title = "Achievement Diary Helper List";
            embed.Description = sb.ToString();

            // send embed reply
            await ReplyAsync(null, false, embed.Build());
        }   

        [Command("diariesadd")]
        [Alias("diaryadd")]
        [RequireUserPermission(GuildPermission.ManageChannels)]
        public async Task AddResponse(string diary, string status)
        {            
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();

            // get user info from the Context
            var user = Context.User;
            
            // check to see if the status is valid
            if (!_validStatus.Contains(status.ToLower()))
            {
                sb.AppendLine($"**Sorry, [{user.Username}], you must specify a valid status.**");
                sb.AppendLine("Valid statuses are:");
                sb.AppendLine();
                foreach (var validStatus in _validStatus)
                {
                    sb.AppendLine($"{validStatus}");
                }       
                embed.Color = new Color(255, 0, 0);         
            }
            else 
            {
                // add diary/color to table
                await _db.AddAsync(new DiaryDevelopmentStatus
                    {
                        DiaryName  = diary,
                        DiaryStatus = status.ToLower()                     
                    }
                );
                // save changes to database
                await _db.SaveChangesAsync();                
                sb.AppendLine();
                sb.AppendLine("**Added Diary:**");
                sb.AppendLine(diary);
                sb.AppendLine();
                sb.AppendLine("**With Status:**");
                sb.AppendLine(status);
                embed.Color = new Color(0, 255, 0);  
            }

            // set embed
            embed.Title = "Achievement Diary Helper Addition";
            embed.Description = sb.ToString();
            
            // send embed reply
            await ReplyAsync(null, false, embed.Build());
        }

        [Command("diariesremove")]
        [Alias("diaryremove")]
        [RequireUserPermission(GuildPermission.ManageChannels)]
        public async Task RemoveDiary(string args)
        {            
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();

            var user = Context.User;

            var diaries = await _db.DiaryDevelopmentStatus.ToListAsync();
            var diaryToRemove = diaries.Where(a => a.DiaryName == args).FirstOrDefault();

            if (diaryToRemove != null)
            {
                _db.Remove(diaryToRemove);
                await _db.SaveChangesAsync();
                sb.AppendLine($"Removed diary -> [{diaryToRemove.DiaryName}]");
            }
            else
            {
                sb.AppendLine($"Could not find diary with id [**{args}**] in the database");
                sb.AppendLine();
                sb.AppendLine($"Perhaps use the {_config["prefix"]}diarieslist command to list out diaries");
            }

            // set embed
            embed.Title = "Diary List - Remove";
            embed.Description = sb.ToString();

            // send embed reply
            await ReplyAsync(null, false, embed.Build());
        } 

        [Command("diariesremovebyid")]
        [Alias("diaryremovebyid")]
        [RequireUserPermission(GuildPermission.ManageChannels)]
        public async Task RemoveDiaryByID(int id)
        {            
            var sb = new StringBuilder();
            var embed = new EmbedBuilder();

            var user = Context.User;

            var diaries = await _db.DiaryDevelopmentStatus.ToListAsync();
            var diaryToRemove = diaries.Where(a => a.DiaryID == id).FirstOrDefault();

            if (diaryToRemove != null)
            {
                _db.Remove(diaryToRemove);
                await _db.SaveChangesAsync();
                sb.AppendLine($"Removed diary -> [{diaryToRemove.DiaryName}]");
            }
            else
            {
                sb.AppendLine($"Did not find diary with id [**{id}**] in the database");
                sb.AppendLine($"Perhaps use the {_config["prefix"]}diarieslistbyid command to list out Achievement Diary Helpers by their ID.");

                foreach (var diary in diaries)
                {
                    var diaryListByIDEmbed = new EmbedFieldBuilder()
                        .WithName($"{diary.DiaryName}")
                        .WithValue($"{diary.DiaryID}")
                        .WithIsInline(true);

                    embed.AddField(diaryListByIDEmbed);          
                }
            }

            // set embed
            embed.Title = "Diary List - Remove";
            embed.WithThumbnailUrl("https://cdn.discordapp.com/attachments/854379120624271380/881270915894247444/Achievement_Diaries.png" ?? Context.Client.CurrentUser.GetAvatarUrl());
            embed.WithFooter($"{_config["EmbedFooter"]}");
            embed.Description = sb.ToString();

            // send embed reply
            await ReplyAsync(null, false, embed.Build());
        } 
    }
} 