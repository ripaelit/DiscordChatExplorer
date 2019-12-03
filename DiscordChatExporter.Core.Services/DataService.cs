﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DiscordChatExporter.Core.Models;
using DiscordChatExporter.Core.Services.Exceptions;
using DiscordChatExporter.Core.Services.Internal;
using Newtonsoft.Json.Linq;
using Polly;

namespace DiscordChatExporter.Core.Services
{
    public partial class DataService : IDisposable
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly IAsyncPolicy<HttpResponseMessage> _httpPolicy;

        public DataService()
        {
            _httpClient.BaseAddress = new Uri("https://discordapp.com/api/v6");

            // Discord seems to always respond 429 on our first request with unreasonable wait time (10+ minutes).
            // For that reason the policy will start respecting their retry-after header only after Nth failed response.
            _httpPolicy = Policy
                .HandleResult<HttpResponseMessage>(m => m.StatusCode == HttpStatusCode.TooManyRequests)
                .OrResult(m => m.StatusCode >= HttpStatusCode.InternalServerError)
                .WaitAndRetryAsync(6,
                    (i, result, ctx) =>
                    {
                        if (i <= 3)
                            return TimeSpan.FromSeconds(2 * i);

                        if (i <= 5)
                            return TimeSpan.FromSeconds(5 * i);

                        return result.Result.Headers.RetryAfter.Delta ?? TimeSpan.FromSeconds(10 * i);
                    },
                    (response, timespan, retryCount, context) => Task.CompletedTask);
        }

        private async Task<JToken> GetApiResponseAsync(AuthToken token, string route)
        {
            using var response = await _httpPolicy.ExecuteAsync(async () =>
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, route);

                request.Headers.Authorization = token.Type == AuthTokenType.Bot
                    ? new AuthenticationHeaderValue("Bot", token.Value)
                    : new AuthenticationHeaderValue(token.Value);

                return await _httpClient.SendAsync(request);
            });

            // We throw our own exception here because default one doesn't have status code
            if (!response.IsSuccessStatusCode)
                throw new HttpErrorStatusCodeException(response.StatusCode, response.ReasonPhrase);

            var jsonRaw = await response.Content.ReadAsStringAsync();
            return JToken.Parse(jsonRaw);
        }

        public async Task<Guild> GetGuildAsync(AuthToken token, string guildId)
        {
            // Special case for direct messages pseudo-guild
            if (guildId == Guild.DirectMessages.Id)
                return Guild.DirectMessages;

            var response = await GetApiResponseAsync(token, $"guilds/{guildId}");
            var guild = ParseGuild(response);

            return guild;
        }

        public async Task<Channel> GetChannelAsync(AuthToken token, string channelId)
        {
            var response = await GetApiResponseAsync(token, $"channels/{channelId}");
            var channel = ParseChannel(response);

            return channel;
        }

        public async IAsyncEnumerable<Guild> EnumerateUserGuildsAsync(AuthToken token)
        {
            var afterId = "";

            while (true)
            {
                var route = "users/@me/guilds?limit=100";
                if (!string.IsNullOrWhiteSpace(afterId))
                    route += $"&after={afterId}";

                var response = await GetApiResponseAsync(token, route);

                if (!response.HasValues)
                    yield break;

                foreach (var guild in response.Select(ParseGuild))
                {
                    yield return guild;
                    afterId = guild.Id;
                }
            }
        }

        public Task<IReadOnlyList<Guild>> GetUserGuildsAsync(AuthToken token) => EnumerateUserGuildsAsync(token).AggregateAsync();

        public async Task<IReadOnlyList<Channel>> GetDirectMessageChannelsAsync(AuthToken token)
        {
            var response = await GetApiResponseAsync(token, "users/@me/channels");
            var channels = response.Select(ParseChannel).ToArray();

            return channels;
        }

        public async Task<IReadOnlyList<Channel>> GetGuildChannelsAsync(AuthToken token, string guildId)
        {
            var response = await GetApiResponseAsync(token, $"guilds/{guildId}/channels");
            var channels = response.Select(ParseChannel).ToArray();

            return channels;
        }

        public async Task<IReadOnlyList<Role>> GetGuildRolesAsync(AuthToken token, string guildId)
        {
            var response = await GetApiResponseAsync(token, $"guilds/{guildId}/roles");
            var roles = response.Select(ParseRole).ToArray();

            return roles;
        }

        private async Task<Message> GetLastMessageAsync(AuthToken token, string channelId, DateTimeOffset? before = null)
        {
            var route = $"channels/{channelId}/messages?limit=1";
            if (before != null)
                route += $"&before={before.Value.ToSnowflake()}";

            var response = await GetApiResponseAsync(token, route);

            return response.Select(ParseMessage).FirstOrDefault();
        }

        public async IAsyncEnumerable<Message> EnumerateMessagesAsync(AuthToken token, string channelId,
            DateTimeOffset? after = null, DateTimeOffset? before = null, IProgress<double>? progress = null)
        {
            // Get the last message
            var lastMessage = await GetLastMessageAsync(token, channelId, before);

            // If the last message doesn't exist or it's outside of range - return
            if (lastMessage == null || lastMessage.Timestamp < after)
            {
                progress?.Report(1);
                yield break;
            }

            // Get other messages
            var firstMessage = default(Message);
            var offsetId = after?.ToSnowflake() ?? "0";
            while (true)
            {
                // Get message batch
                var route = $"channels/{channelId}/messages?limit=100&after={offsetId}";
                var response = await GetApiResponseAsync(token, route);

                // Parse
                var messages = response
                    .Select(ParseMessage)
                    .Reverse() // reverse because messages appear newest first
                    .ToArray();

                // Break if there are no messages (can happen if messages are deleted during execution)
                if (!messages.Any())
                    break;

                // Trim messages to range (until last message)
                var messagesInRange = messages
                    .TakeWhile(m => m.Id != lastMessage.Id && m.Timestamp < lastMessage.Timestamp)
                    .ToArray();

                // Yield messages
                foreach (var message in messagesInRange)
                {
                    // Set first message if it's not set
                    firstMessage ??= message;

                    // Report progress (based on the time range of parsed messages compared to total)
                    progress?.Report((message.Timestamp - firstMessage.Timestamp).TotalSeconds /
                                     (lastMessage.Timestamp - firstMessage.Timestamp).TotalSeconds);

                    yield return message;
                    offsetId = message.Id;
                }

                // Break if messages were trimmed (which means the last message was encountered)
                if (messagesInRange.Length != messages.Length)
                    break;
            }

            // Yield last message
            yield return lastMessage;

            // Report progress
            progress?.Report(1);
        }

        public Task<IReadOnlyList<Message>> GetMessagesAsync(AuthToken token, string channelId,
            DateTimeOffset? after = null, DateTimeOffset? before = null, IProgress<double>? progress = null) =>
            EnumerateMessagesAsync(token, channelId, after, before, progress).AggregateAsync();

        public async Task<Mentionables> GetMentionablesAsync(AuthToken token, string guildId,
            IEnumerable<Message> messages)
        {
            // Get channels and roles
            var channels = guildId != Guild.DirectMessages.Id
                ? await GetGuildChannelsAsync(token, guildId)
                : Array.Empty<Channel>();
            var roles = guildId != Guild.DirectMessages.Id
                ? await GetGuildRolesAsync(token, guildId)
                : Array.Empty<Role>();

            // Get users
            var userMap = new Dictionary<string, User>();
            foreach (var message in messages)
            {
                // Author
                userMap[message.Author.Id] = message.Author;

                // Mentioned users
                foreach (var mentionedUser in message.MentionedUsers)
                    userMap[mentionedUser.Id] = mentionedUser;
            }

            var users = userMap.Values.ToArray();

            return new Mentionables(users, channels, roles);
        }

        public async Task<ChatLog> GetChatLogAsync(AuthToken token, Guild guild, Channel channel,
            DateTimeOffset? after = null, DateTimeOffset? before = null, IProgress<double>? progress = null)
        {
            // Get messages
            var messages = await GetMessagesAsync(token, channel.Id, after, before, progress);

            // Get mentionables
            var mentionables = await GetMentionablesAsync(token, guild.Id, messages);

            return new ChatLog(guild, channel, after, before, messages, mentionables);
        }

        public async Task<ChatLog> GetChatLogAsync(AuthToken token, Channel channel,
            DateTimeOffset? after = null, DateTimeOffset? before = null, IProgress<double>? progress = null)
        {
            // Get guild
            var guild = !string.IsNullOrWhiteSpace(channel.GuildId)
                ? await GetGuildAsync(token, channel.GuildId)
                : Guild.DirectMessages;

            // Get the chat log
            return await GetChatLogAsync(token, guild, channel, after, before, progress);
        }

        public void Dispose() => _httpClient.Dispose();
    }
}