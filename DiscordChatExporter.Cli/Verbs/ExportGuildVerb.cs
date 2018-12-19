﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using DiscordChatExporter.Cli.Verbs.Options;
using DiscordChatExporter.Core.Exceptions;
using DiscordChatExporter.Core.Helpers;
using DiscordChatExporter.Core.Models;
using DiscordChatExporter.Core.Services;
using Tyrrrz.Extensions;

namespace DiscordChatExporter.Cli.Verbs
{
    public class ExportGuildVerb : Verb<ExportGuildOptions>
    {
        public ExportGuildVerb(ExportGuildOptions options)
            : base(options)
        {
        }

        public override async Task ExecuteAsync()
        {
            // Get services
            var settingsService = Container.Instance.Get<SettingsService>();
            var dataService = Container.Instance.Get<DataService>();
            var exportService = Container.Instance.Get<ExportService>();

            // Configure settings
            if (Options.DateFormat.IsNotBlank())
                settingsService.DateFormat = Options.DateFormat;
            if (Options.MessageGroupLimit > 0)
                settingsService.MessageGroupLimit = Options.MessageGroupLimit;

            // Get channels
            var channels = await dataService.GetGuildChannelsAsync(Options.GetToken(), Options.GuildId);

            // Filter and order channels
            channels = channels.Where(c => c.Type == ChannelType.GuildTextChat).OrderBy(c => c.Name).ToArray();

            // Loop through channels
            foreach (var channel in channels)
            {
                try
                {
                    // Print current channel name
                    Console.WriteLine($"Exporting chat from [{channel.Name}]...");

                    // Get chat log
                    var chatLog = await dataService.GetChatLogAsync(Options.GetToken(), channel,
                        Options.After, Options.Before);

                    // Generate file path if not set or is a directory
                    var filePath = Options.FilePath;
                    if (filePath == null || filePath.EndsWith("/") || filePath.EndsWith("\\"))
                    {
                        // Generate default file name
                        var defaultFileName = ExportHelper.GetDefaultExportFileName(Options.ExportFormat, chatLog.Guild,
                            chatLog.Channel, Options.After, Options.Before);

                        // Append the file name to the file path
                        filePath += defaultFileName;
                    }

                    // Export
                    exportService.ExportChatLog(chatLog, filePath, Options.ExportFormat, Options.PartitionLimit);

                    // Print result
                    Console.WriteLine($"Exported chat to [{filePath}]");
                }
                catch (HttpErrorStatusCodeException ex) when (ex.StatusCode == HttpStatusCode.Forbidden)
                {
                    Console.Error.WriteLine("You don't have access to this channel");
                }
                catch (HttpErrorStatusCodeException ex) when (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    Console.Error.WriteLine("This channel doesn't exist");
                }
            }
        }
    }
}