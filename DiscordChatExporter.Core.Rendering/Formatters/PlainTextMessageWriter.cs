﻿using System.IO;
using System.Threading.Tasks;
using DiscordChatExporter.Core.Models;
using DiscordChatExporter.Core.Rendering.Logic;

namespace DiscordChatExporter.Core.Rendering.Formatters
{
    public class PlainTextMessageWriter : MessageWriterBase
    {
        private long _messageCount;

        public PlainTextMessageWriter(TextWriter writer, RenderContext context)
            : base(writer, context)
        {
        }

        public override async Task WritePreambleAsync()
        {
            await Writer.WriteLineAsync(PlainTextRenderingLogic.FormatPreamble(Context));
        }

        public override async Task WriteMessageAsync(Message message)
        {
            await Writer.WriteLineAsync(PlainTextRenderingLogic.FormatMessage(Context, message));
            await Writer.WriteLineAsync();

            _messageCount++;
        }

        public override async Task WritePostambleAsync()
        {
            await Writer.WriteLineAsync();
            await Writer.WriteLineAsync(PlainTextRenderingLogic.FormatPostamble(_messageCount));
        }
    }
}