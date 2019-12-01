﻿using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CliFx.Attributes;
using CliFx.Services;
using DiscordChatExporter.Core.Models;
using DiscordChatExporter.Core.Services;
using DiscordChatExporter.Core.Services.Exceptions;

namespace DiscordChatExporter.Cli.Commands
{
    [Command("exportguild", Description = "Export all channels within specified guild.")]
    public class ExportGuildCommand : ExportCommandBase
    {
        [CommandOption("guild", 'g', IsRequired = true, Description = "Guild ID.")]
        public string GuildId { get; set; }

        public ExportGuildCommand(SettingsService settingsService, DataService dataService, ExportService exportService)
            : base(settingsService, dataService, exportService)
        {
        }

        public override async Task ExecuteAsync(IConsole console)
        {
            // Get channels
            var channels = await DataService.GetGuildChannelsAsync(GetToken(), GuildId);

            // Filter and order channels
            channels = channels.Where(c => c.Type.IsExportable()).OrderBy(c => c.Name).ToArray();

            // Loop through channels
            foreach (var channel in channels)
            {
                try
                {
                    await ExportChannelAsync(console, channel);
                }
                catch (HttpErrorStatusCodeException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
                {
                    console.Error.WriteLine("You don't have access to this channel.");
                }
                catch (HttpErrorStatusCodeException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    console.Error.WriteLine("This channel doesn't exist.");
                }
            }
        }
    }
}