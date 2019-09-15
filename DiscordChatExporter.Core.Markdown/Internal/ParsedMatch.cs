﻿namespace DiscordChatExporter.Core.Markdown.Internal
{
    internal class ParsedMatch<T>
    {
        public StringPart StringPart { get; }

        public T Value { get; }

        public ParsedMatch(StringPart stringPart, T value)
        {
            StringPart = stringPart;
            Value = value;
        }
    }
}