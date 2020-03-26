﻿using System.Drawing;

namespace DiscordChatExporter.Core.Models
{
    // https://discordapp.com/developers/docs/topics/permissions#role-object

    public partial class Role : IHasId
    {
        public string Id { get; }

        public string Name { get; }

        public Color Color { get; }

        public string ColorAsHex => $"#{Color.ToArgb() & 0xffffff:X6}";

        public string ColorAsRgb => $"{Color.R}, {Color.G}, {Color.B}";

        public int Position { get; }

        public Role(string id, string name, Color color, int position)
        {
            Id = id;
            Name = name;
            Color = color;
            Position = position;
        }

        public override string ToString() => Name;
    }

    public partial class Role
    {
        public static Role CreateDeletedRole(string id) => new Role(id, "deleted-role", Color.Black, -1);
    }
}