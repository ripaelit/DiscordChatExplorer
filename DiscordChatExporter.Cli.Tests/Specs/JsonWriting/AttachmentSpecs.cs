﻿using System.Linq;
using System.Threading.Tasks;
using DiscordChatExporter.Cli.Tests.Fixtures;
using DiscordChatExporter.Cli.Tests.TestData;
using DiscordChatExporter.Core.Discord;
using FluentAssertions;
using Xunit;

namespace DiscordChatExporter.Cli.Tests.Specs.JsonWriting;

public record AttachmentSpecs(ExportWrapperFixture ExportWrapper) : IClassFixture<ExportWrapperFixture>
{
    [Fact]
    public async Task Message_with_a_generic_attachment_is_rendered_correctly()
    {
        // Act
        var message = await ExportWrapper.GetMessageAsJsonAsync(
            ChannelIds.AttachmentTestCases,
            Snowflake.Parse("885587844989612074")
        );

        var attachments = message.GetProperty("attachments").EnumerateArray().ToArray();

        // Assert
        message.GetProperty("content").GetString().Should().Be("Generic file attachment");

        attachments.Should().HaveCount(1);
        attachments.Single().GetProperty("url").GetString().Should().StartWithEquivalentOf(
            "https://cdn.discordapp.com/attachments/885587741654536192/885587844964417596/Test.txt"
        );
        attachments.Single().GetProperty("fileName").GetString().Should().Be("Test.txt");
        attachments.Single().GetProperty("fileSizeBytes").GetInt64().Should().Be(11);
    }

    [Fact]
    public async Task Message_with_an_image_attachment_is_rendered_correctly()
    {
        // Act
        var message = await ExportWrapper.GetMessageAsJsonAsync(
            ChannelIds.AttachmentTestCases,
            Snowflake.Parse("885654862656843786")
        );

        var attachments = message.GetProperty("attachments").EnumerateArray().ToArray();

        // Assert
        message.GetProperty("content").GetString().Should().Be("Image attachment");

        attachments.Should().HaveCount(1);
        attachments.Single().GetProperty("url").GetString().Should().StartWithEquivalentOf(
            "https://cdn.discordapp.com/attachments/885587741654536192/885654862430359613/bird-thumbnail.png"
        );
        attachments.Single().GetProperty("fileName").GetString().Should().Be("bird-thumbnail.png");
        attachments.Single().GetProperty("fileSizeBytes").GetInt64().Should().Be(466335);
    }

    [Fact]
    public async Task Message_with_a_video_attachment_is_rendered_correctly()
    {
        // Act
        var message = await ExportWrapper.GetMessageAsJsonAsync(
            ChannelIds.AttachmentTestCases,
            Snowflake.Parse("885655761919836171")
        );

        var attachments = message.GetProperty("attachments").EnumerateArray().ToArray();

        // Assert
        message.GetProperty("content").GetString().Should().Be("Video attachment");

        attachments.Should().HaveCount(1);
        attachments.Single().GetProperty("url").GetString().Should().StartWithEquivalentOf(
            "https://cdn.discordapp.com/attachments/885587741654536192/885655761512968233/file_example_MP4_640_3MG.mp4"
        );
        attachments.Single().GetProperty("fileName").GetString().Should().Be("file_example_MP4_640_3MG.mp4");
        attachments.Single().GetProperty("fileSizeBytes").GetInt64().Should().Be(3114374);
    }

    [Fact]
    public async Task Message_with_an_audio_attachment_is_rendered_correctly()
    {
        // Act
        var message = await ExportWrapper.GetMessageAsJsonAsync(
            ChannelIds.AttachmentTestCases,
            Snowflake.Parse("885656175620808734")
        );

        var attachments = message.GetProperty("attachments").EnumerateArray().ToArray();

        // Assert
        message.GetProperty("content").GetString().Should().Be("Audio attachment");

        attachments.Should().HaveCount(1);
        attachments.Single().GetProperty("url").GetString().Should().StartWithEquivalentOf(
            "https://cdn.discordapp.com/attachments/885587741654536192/885656175348187146/file_example_MP3_1MG.mp3"
        );
        attachments.Single().GetProperty("fileName").GetString().Should().Be("file_example_MP3_1MG.mp3");
        attachments.Single().GetProperty("fileSizeBytes").GetInt64().Should().Be(1087849);
    }
}