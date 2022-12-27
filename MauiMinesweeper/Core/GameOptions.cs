using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMinesweeper.Core
{
    internal class GameOptions
    {
        internal int Width { get; init; }
        internal int Height { get; init; }
        internal int MineCount { get; init; }

        internal static GameOptions Easy => new GameOptions() { Width = 9, Height = 9, MineCount = 10 };
        internal static GameOptions Medium => new GameOptions() { Width = 16, Height = 16, MineCount = 40 };
        internal static GameOptions Hard => new GameOptions() { Width = 30, Height = 16, MineCount = 99 };

        private GameOptions() { }
    }
}
