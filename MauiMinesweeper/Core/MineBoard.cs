using MauiMinesweeper.Controls;
using MauiMinesweeper.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiMinesweeper.Core
{
    internal class MineBoard
    {
        private static readonly Random random = new Random();

        private readonly GameOptions currentGameOptions;
        private bool isFirstCellRevealed = false;

        private int boardWidth => this.currentGameOptions.Width;
        private int boardHeight => this.currentGameOptions.Height;

        internal MineCell[,] MineCells { get; private set; }
        internal int UnmarkedMineCount { get; private set; }

        internal event Action UnmarkedMineCountChanged;
        internal event Action GameOver;

        internal MineBoard(GameOptions options)
        {
            this.currentGameOptions = options;
            this.MineCells = new MineCell[options.Width, options.Height];
            this.UnmarkedMineCount = options.MineCount;

            for (int y = 0; y < options.Height; y++)
            {
                for (int x = 0; x < options.Width; x++)
                {
                    var newCell = new MineCell();
                    newCell.Reset(x, y);
                    newCell.Revealed += this.OnMineCellRevealed;
                    newCell.Marked += this.OnMineCellMarked;
                    newCell.Unmarked += this.OnMineCellUnmarked;
                    this.MineCells[x, y] = newCell;
                }
            }
        }

        internal IEnumerable<MineCell> GetAdjacentCells(int x, int y)
        {
            for(var innerY = -1; innerY <= 1; innerY++)
            {
                for(var innerX = -1;innerX <= 1; innerX++)
                {
                    var currentY = y + innerY;
                    var currentX = x + innerX;

                    if (currentY < 0 || currentX < 0 || currentY >= boardHeight || currentX >= boardWidth)
                        continue;

                    if (currentX == x && currentY == y)
                        continue;

                    yield return this.MineCells[currentX, currentY];
                }
            }
        }

        private IEnumerable<MineCell> GetAllCells()
        {
            for(var y = 0; y < this.currentGameOptions.Height;y++)
                for(var x = 0; x < this.currentGameOptions.Width;x++)
                    yield return this.MineCells[x, y];
        }

        private void OnMineCellRevealed(MineCell cell)
        {
            if(!this.isFirstCellRevealed)
            {
                GenerateMines(cell.X, cell.Y);
                isFirstCellRevealed = true;
            }

            if(cell.HasMine)
            {
                this.GameOver?.Invoke();
                return;
            }

            if(UnmarkedMineCount == 0 && GetAllCells().Where(i => i.HasMine).All(i => i.IsMarked))
            {
                this.GameOver?.Invoke();
                return;
            }

            if (!cell.HasMine && cell.AdjacentMineCount == 0)
            {
                foreach (var adjacentCell in GetAdjacentCells(cell.X, cell.Y))
                {
                    adjacentCell.Reveal();
                }
            }
        }

        private void OnMineCellMarked(MineCell _)
        {
            this.UnmarkedMineCount--;
            this.UnmarkedMineCountChanged?.Invoke();
        }

        private void OnMineCellUnmarked(MineCell _)
        {
            this.UnmarkedMineCount++;
            this.UnmarkedMineCountChanged?.Invoke();
        }

        // Wait until first cell is revealed so we can ensure there is a safe area to start with
        private void GenerateMines(int startX, int startY)
        {
            var safeStartX = startX - 1;
            var safeEndX = startX + 1;
            var safeStartY = startY - 1;
            var safeEndY = startY + 1;

            bool IsInSafeArea(int x, int y) => x >= safeStartX && x <= safeEndX && y >= safeStartY && y <= safeEndY;

            for(int i = 0; i < this.currentGameOptions.MineCount;i++)
            {
                while(true)
                {
                    var newX = random.Next(0, this.boardWidth);
                    var newY = random.Next(0, this.boardHeight);

                    var cell = this.MineCells[newX, newY];

                    if (cell.HasMine)
                        continue;

                    // Is the selected cell part of the 'safe' starting area
                    if (IsInSafeArea(newX, newY))
                        continue;

                    cell.HasMine = true;
                    foreach (var adjacentCell in GetAdjacentCells(newX, newY))
                        adjacentCell.IncrementAdjacentMineCount();

                    break;
                }
            }
        }
    }
}
