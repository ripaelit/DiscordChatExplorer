﻿using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using DiscordChatExporter.Cli.Tests.Fixtures;
using DiscordChatExporter.Cli.Tests.TestData;
using DiscordChatExporter.Core.Discord;
using FluentAssertions;
using Xunit;

namespace DiscordChatExporter.Cli.Tests.Specs.HtmlWriting;

public class AttachmentSpecs : IClassFixture<ExportWrapperFixture>
{
    private readonly ExportWrapperFixture _exportWrapper;

    public AttachmentSpecs(ExportWrapperFixture exportWrapper)
    {
        _exportWrapper = exportWrapper;
    }

    [Fact]
    public async Task Message_with_a_generic_attachment_is_rendered_correctly()
    {
        // Act
        var message = await _exportWrapper.GetMessageAsHtmlAsync(
            ChannelIds.AttachmentTestCases,
            Snowflake.Parse("885587844989612074")
        );

        // Assert
        message.Text().Should().ContainAll(
            "Generic file attachment",
            "Test.txt",
            "11 bytes"
        );

        message
            .QuerySelectorAll("a")
            .Select(e => e.GetAttribute("href"))
            .Should()
            .Contain(
                "https://cdn.discordapp.com/attachments/885587741654536192/885587844964417596/Test.txt"
            );
    }

    [Fact]
    public async Task Message_with_an_image_attachment_is_rendered_correctly()
    {
        // Act
        var message = await _exportWrapper.GetMessageAsHtmlAsync(
            ChannelIds.AttachmentTestCases,
            Snowflake.Parse("885654862656843786")
        );

        // Assert
        message.Text().Should().Contain("Image attachment");

        message
            .QuerySelectorAll("img")
            .Select(e => e.GetAttribute("src"))
            .Should()
            .Contain(
                "https://cdn.discordapp.com/attachments/885587741654536192/885654862430359613/bird-thumbnail.png"
            );
    }

    [Fact]
    public async Task Message_with_a_video_attachment_is_rendered_correctly()
    {
        // https://github.com/Tyrrrz/DiscordChatExporter/issues/333

        // Act
        var message = await _exportWrapper.GetMessageAsHtmlAsync(
            ChannelIds.AttachmentTestCases,
            Snowflake.Parse("885655761919836171")
        );

        // Assert
        message.Text().Should().Contain("Video attachment");

        var videoUrl = message.QuerySelector("video source")?.GetAttribute("src");
        videoUrl.Should().Be(
            "https://cdn.discordapp.com/attachments/885587741654536192/885655761512968233/file_example_MP4_640_3MG.mp4"
        );
    }

    [Fact]
    public async Task Message_with_an_audio_attachment_is_rendered_correctly()
    {
        // https://github.com/Tyrrrz/DiscordChatExporter/issues/333

        // Act
        var message = await _exportWrapper.GetMessageAsHtmlAsync(
            ChannelIds.AttachmentTestCases,
            Snowflake.Parse("885656175620808734")
        );

        // Assert
        message.Text().Should().Contain("Audio attachment");

        var audioUrl = message.QuerySelector("audio source")?.GetAttribute("src");
        audioUrl.Should().Be(
            "https://cdn.discordapp.com/attachments/885587741654536192/885656175348187146/file_example_MP3_1MG.mp3"
        );
    }
}