﻿using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DiscordChatExporter.Core.Utils;

public static class PathEx
{
    private static readonly HashSet<char> InvalidFileNameChars = new(Path.GetInvalidFileNameChars());

    public static string EscapeFileName(string path)
    {
        var buffer = new StringBuilder(path.Length);

        foreach (var c in path)
            buffer.Append(!InvalidFileNameChars.Contains(c) ? c : '_');

        return buffer.ToString();
    }
}