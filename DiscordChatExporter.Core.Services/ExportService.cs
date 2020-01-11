﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DiscordChatExporter.Core.Models;
using DiscordChatExporter.Core.Models.Exceptions;
using DiscordChatExporter.Core.Rendering;
using DiscordChatExporter.Core.Services.Logic;
using Tyrrrz.Extensions;

namespace DiscordChatExporter.Core.Services
{
    public partial class ExportService
    {
        private readonly SettingsService _settingsService;
        private readonly DataService _dataService;

        public ExportService(SettingsService settingsService, DataService dataService)
        {
            _settingsService = settingsService;
            _dataService = dataService;
        }

        public async Task ExportChatLogAsync(AuthToken token, Guild guild, Channel channel,
            string outputPath, ExportFormat format, int? partitionLimit,
            DateTimeOffset? after = null, DateTimeOffset? before = null, IProgress<double>? progress = null)
        {
            // Get base file path from output path
            var baseFilePath = GetFilePathFromOutputPath(outputPath, format, guild, channel, after, before);

            // Create options
            var options = new RenderOptions(baseFilePath, format, partitionLimit);

            // Create context
            var mentionableUsers = new HashSet<User>(IdBasedEqualityComparer.Instance);
            var mentionableChannels = await _dataService.GetGuildChannelsAsync(token, guild.Id);
            var mentionableRoles = await _dataService.GetGuildRolesAsync(token, guild.Id);

            var context = new RenderContext
            (
                guild, channel, after, before, _settingsService.DateFormat,
                mentionableUsers, mentionableChannels, mentionableRoles
            );

            // Create renderer
            await using var renderer = new MessageRenderer(options, context);

            // Render messages
            var renderedAnything = false;
            await foreach (var message in _dataService.GetMessagesAsync(token, channel.Id, after, before, progress))
            {
                // Add encountered users to the list of mentionable users
                mentionableUsers.Add(message.Author);
                mentionableUsers.AddRange(message.MentionedUsers);

                // Render message
                await renderer.RenderMessageAsync(message);
                renderedAnything = true;
            }

            // Throw if no messages were rendered
            if (!renderedAnything)
                throw new DomainException($"Channel [{channel.Name}] contains no messages for specified period");
        }
    }

    public partial class ExportService
    {
        private static string GetFilePathFromOutputPath(string outputPath, ExportFormat format, Guild guild, Channel channel,
            DateTimeOffset? after, DateTimeOffset? before)
        {
            // Output is a directory
            if (Directory.Exists(outputPath) || string.IsNullOrWhiteSpace(Path.GetExtension(outputPath)))
            {
                var fileName = ExportLogic.GetDefaultExportFileName(format, guild, channel, after, before);
                return Path.Combine(outputPath, fileName);
            }

            // Output is a file
            return outputPath;
        }
    }
}