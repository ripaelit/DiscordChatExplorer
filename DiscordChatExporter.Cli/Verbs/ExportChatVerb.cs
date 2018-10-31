﻿using System;
using System.IO;
using System.Threading.Tasks;
using DiscordChatExporter.Cli.Verbs.Options;
using DiscordChatExporter.Core.Models;
using DiscordChatExporter.Core.Services;
using Tyrrrz.Extensions;

namespace DiscordChatExporter.Cli.Verbs
{
    public class ExportChatVerb : Verb<ExportChatOptions>
    {
        public ExportChatVerb(ExportChatOptions options)
            : base(options)
        {
        }

        public override async Task ExecuteAsync()
        {
            // Get services
            var container = new Container();
            var settingsService = container.Resolve<ISettingsService>();
            var chatLogService = container.Resolve<IChatLogService>();
            var exportService = container.Resolve<IExportService>();

            // Configure settings
            if (Options.DateFormat.IsNotBlank())
                settingsService.DateFormat = Options.DateFormat;
            if (Options.MessageGroupLimit > 0)
                settingsService.MessageGroupLimit = Options.MessageGroupLimit;

            // Get chat log
            var chatLog = await chatLogService.GetChatLogAsync(Options.GetToken(), Options.ChannelId, 
                Options.After, Options.Before);

            // Generate file path if not set
            var filePath = Options.FilePath;
            if (filePath == null || filePath.EndsWith("/") || filePath.EndsWith("\\"))
            {
                filePath += $"{chatLog.Guild.Name} - {chatLog.Channel.Name}.{Options.ExportFormat.GetFileExtension()}"
                    .Replace(Path.GetInvalidFileNameChars(), '_');
            }

            // Export
            exportService.Export(chatLog, filePath, Options.ExportFormat);

            // Print result
            Console.WriteLine($"Exported chat to [{filePath}]");
        }
    }
}