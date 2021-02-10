# DiscordChatExporter

[![Build](https://github.com/Tyrrrz/DiscordChatExporter/workflows/CI/badge.svg?branch=master)](https://github.com/Tyrrrz/DiscordChatExporter/actions)
[![Release](https://img.shields.io/github/release/Tyrrrz/DiscordChatExporter.svg)](https://github.com/Tyrrrz/DiscordChatExporter/releases)
[![Downloads](https://img.shields.io/github/downloads/Tyrrrz/DiscordChatExporter/total.svg)](https://github.com/Tyrrrz/DiscordChatExporter/releases)
[![Donate](https://img.shields.io/badge/donate-$$$-purple.svg)](https://tyrrrz.me/donate)
[![Extra Services](https://img.shields.io/badge/extra%20services-xs:code-blue.svg)](https://xscode.com/Tyrrrz/DiscordChatExporter)

✅ **Project status: active**.

DiscordChatExporter can be used to export message history from a [Discord](https://discord.com) channel to a file.
It works with direct messages, group messages, and server channels, and supports Discord's dialect of markdown as well as all other rich media features.

❓ **If you have questions or issues, please refer to the [wiki](https://github.com/Tyrrrz/DiscordChatExporter/wiki)**.

## Download

This application comes in two flavors: graphical user interface (**GUI**) and command line interface (**CLI**).
The following table lists all available download options:

<table>
  <thead>
    <tr>
      <th></th>
      <th>Downloads</th>
      <th>Supported OS</th>
      <th>Requirements</th>
    </tr>
  </thead>
  <tbody>
    <tr>
      <td><b>GUI</b></td>
      <td>
        <ul>
          <li>🟢 <b><a href="https://github.com/Tyrrrz/DiscordChatExporter/releases/latest">Stable release</a></b> (<code>DiscordChatExporter.zip</code>)</li>
          <li>🟠 <a href="https://github.com/Tyrrrz/DiscordChatExporter/actions?query=workflow%3ACI">CI build</a> (<code>DiscordChatExporter.zip</code>)</li>
        </ul>
      </td>
      <td>
        <ul>
          <li>Windows 7+</li>
        </ul>
      </td>
      <td>
        <div>.NET v3.1 Desktop Runtime</div>
        <ul>
          <li><a href="https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-desktop-3.1.0-windows-x64-installer">Windows x64</a></li>
          <li><a href="https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-desktop-3.1.0-windows-x86-installer">Windows x86</a></li>
        </ul>
      </td>
    </tr>
    <tr>
      <td><b>CLI</b></td>
      <td>
        <ul>
          <li>🟢 <b><a href="https://github.com/Tyrrrz/DiscordChatExporter/releases/latest">Stable release</a></b> (<code>DiscordChatExporter.CLI.zip</code>)</li>
          <li>🟠 <a href="https://github.com/Tyrrrz/DiscordChatExporter/actions?query=workflow%3ACI">CI build</a> (<code>DiscordChatExporter.CLI.zip</code>)</li>
          <li>🐋 <a href="https://hub.docker.com/r/tyrrrz/discordchatexporter">Docker</a> (<code>tyrrrz/discordchatexporter</code>) 🚀</li>
          <li>📦 <a href="https://aur.archlinux.org/packages/discord-chat-exporter-cli">AUR</a> (<code>discord-chat-exporter-cli</code>) 🚀 🦄</li>
        </ul>
      </td>
      <td>
        <ul>
          <li>Windows 7+</li>
          <li>macOS X 10.11+</li>
          <li>Linux</li>
        </ul>
      </td>
      <td>
        <div>.NET v3.1 Base Runtime</div>
        <ul>
          <li><a href="https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-desktop-3.1.0-windows-x64-installer">Windows x64</a></li>
          <li><a href="https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-desktop-3.1.0-windows-x86-installer">Windows x86</a></li>
          <li><a href="https://dotnet.microsoft.com/download/dotnet-core/thank-you/runtime-3.1.0-macos-x64-installer">macOS x64</a></li>
          <li><a href="https://docs.microsoft.com/en-us/dotnet/core/install/linux">Linux (find your distribution)</a></li>
        </ul>
      </td>
    </tr>
  </tbody>
</table>

> Note: DiscordChatExporter requires **.NET v3.1 Runtime** in order to work!
Refer to the **Requirements** column in the above table to download the appropriate installer for your system.

- 🚀 - .NET Runtime is already embedded or installed automatically
- 🦄 - Community-maintained resource

## Features

- Graphical user interface (Windows)
- Command line interface (Windows, Linux, macOS)
- Works with both user and bot tokens
- Exports on the fly without buffering messages in memory
- Allows exporting messages in a specified range
- Supports file partitioning based on message count
- Uses custom markdown parser compatible with Discord syntax
- Handles all rich media features, including attachments, embeds, emojis, etc
- Renders to HTML (dark & light), TXT, CSV, JSON

## Screenshots

![channel list](.screenshots/list.png)
![rendered output](.screenshots/output.png)
