using System.Text.Json;
using JsonExtensions.Reading;

namespace DiscordChatExporter.Core.Discord.Data.Embeds
{
    // https://discord.com/developers/docs/resources/channel#embed-object-embed-author-structure
    public record EmbedAuthor(
        string? Name,
        string? Url,
        string? IconUrl,
        string? IconProxyUrl)
    {
        public static EmbedAuthor Parse(JsonElement json)
        {
            var name = json.GetPropertyOrNull("name")?.GetStringOrNull();
            var url = json.GetPropertyOrNull("url")?.GetStringOrNull();
            var iconUrl = json.GetPropertyOrNull("icon_url")?.GetStringOrNull();
            var iconProxyUrl = json.GetPropertyOrNull("proxy_icon_url")?.GetStringOrNull();

            return new EmbedAuthor(name, url, iconUrl, iconProxyUrl);
        }
    }
}