﻿using System.Threading.Tasks;
using CliFx.Attributes;
using CliFx.Services;
using DiscordChatExporter.Core.Services;

namespace DiscordChatExporter.Cli.Commands
{
    [Command("export", Description = "Export a channel.")]
    public class ExportChannelCommand : ExportCommandBase
    {
        [CommandOption("channel", 'c', IsRequired = true, Description= "Channel ID.")]
        public string ChannelId { get; set; }

        public ExportChannelCommand(SettingsService settingsService, DataService dataService, ExportService exportService)
            : base(settingsService, dataService, exportService)
        {
        }

        public override async Task ExecuteAsync(IConsole console) => await ExportAsync(console, ChannelId);
    }
}