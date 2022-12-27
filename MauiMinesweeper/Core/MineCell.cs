using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMinesweeper.Core
{
    internal class MineCell
    {
        internal int X { get; private set; }
        internal int Y { get; private set; }
        internal bool HasMine { get; set; }
        internal bool IsRevealed { get; private set; }
        internal bool IsMarked { get; private set; }
        internal int AdjacentMineCount { get; private set; }

        internal event Action<MineCell> Revealed;
        internal event Action<MineCell> Marked;
        internal event Action<MineCell> Unmarked;

        internal void Reset(int x, int y)
        {
            this.X = x;
            this.Y = y;
            this.HasMine = false;
            this.IsRevealed = false;
            this.IsMarked = false;
            this.AdjacentMineCount = 0;

            foreach (var d in this.Revealed?.GetInvocationList() ?? Enumerable.Empty<Delegate>())
                this.Revealed -= (Action<MineCell>)d;

            foreach (var d in this.Marked?.GetInvocationList() ?? Enumerable.Empty<Delegate>())
                this.Marked -= (Action<MineCell>)d;

            foreach (var d in this.Unmarked?.GetInvocationList() ?? Enumerable.Empty<Delegate>())
                this.Unmarked -= (Action<MineCell>)d;
        }

        internal void IncrementAdjacentMineCount() => this.AdjacentMineCount++;

        internal void Reveal()
        {
            if (this.IsRevealed || this.IsMarked)
                return;

            this.IsRevealed = true;
            
            this.Revealed?.Invoke(this);
        }

        internal void Mark()
        {
            if (this.IsMarked || this.IsRevealed)
                return;

            this.IsMarked = true;

            this.Marked?.Invoke(this);
        }

        internal void Unmark()
        {
            if (!this.IsMarked || this.IsRevealed)
                return;

            this.IsMarked = false;

            this.Unmarked?.Invoke(this);
        }
    }
}
