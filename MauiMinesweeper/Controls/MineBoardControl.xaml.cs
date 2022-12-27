using MauiMinesweeper.Core;
using MauiMinesweeper.Extensions;
using System.Buffers;

namespace MauiMinesweeper.Controls;

public partial class MineBoardControl : ContentView
{
    private const int cellMargin = 1;

    private MineCellControl[,] mineCellControls;
    private GameOptions currentGameOptions;

    internal MineBoard MineBoard { get; private set; }

    internal event Action GameOver;

    public MineBoardControl()
    {
        InitializeComponent();
    }

    internal void StartNewGame(GameOptions options)
    {
        MineBoardLayout.Children.Clear();
        if (this.MineBoard != null)
            this.MineBoard.GameOver -= this.MineBoardGameOver;

        this.currentGameOptions = options;
        this.MineBoard = new MineBoard(options);
        this.MineBoard.GameOver += this.MineBoardGameOver;

        this.mineCellControls = new MineCellControl[options.Width, options.Height];

        for (int y = 0; y < options.Height; y++)
        {
            for (int x = 0; x < options.Width; x++)
            {
                var cell = this.MineBoard.MineCells[x, y];
                var cellControl = new MineCellControl();
                cellControl.Reset(cell);
                cellControl.DoubleTapped += MineCellControlDoubleTapped;
                this.mineCellControls[x, y] = cellControl;
                MineBoardLayout.Add(cellControl);
            }
        }

        Render();
    }

    private void OnSizeChanged(object sender, EventArgs e) => Render();

    private void Render()
    {
        if (this.mineCellControls == null)
            return;

        var pageWidth = this.Width;
        var pageHeight = this.Height;
        var cellSize = (int)((Math.Min(pageWidth, pageHeight) / Math.Min(currentGameOptions.Width, currentGameOptions.Height)) - (2 * cellMargin));

        var nextX = 0;
        var nextY = 0;
        for (int y = 0; y < currentGameOptions.Height; y++)
        {
            nextX = 0;

            // Before margin
            if (y == 0)
                nextY += cellMargin;

            for (int x = 0; x < currentGameOptions.Width; x++)
            {
                // Before margin
                if (x == 0)
                    nextX += cellMargin;

                var cell = this.mineCellControls[x, y];
                MineBoardLayout.SetLayoutBounds(cell, new Rect() { X = nextX, Y = nextY, Width = cellSize, Height = cellSize });

                // Cell content
                nextX += cellSize;

                // After margin
                nextX += cellMargin;
            }

            // Cell content
            nextY += cellSize;

            // After margin
            nextY += cellMargin;
        }
    }

    private void MineCellControlDoubleTapped(MineCellControl mineCellControl)
    {
        var mineCell = mineCellControl.MineCell;

        if (!mineCell.IsRevealed || mineCell.IsMarked)
            return;

        var adjacentMarkedCount = this.MineBoard.GetAdjacentCells(mineCell.X, mineCell.Y).Count(i => i.IsMarked);

        if (adjacentMarkedCount != mineCell.AdjacentMineCount)
            return;

        foreach (var adjacentCell in this.MineBoard.GetAdjacentCells(mineCell.X, mineCell.Y))
        {
            if (!adjacentCell.IsRevealed && !adjacentCell.IsMarked)
                adjacentCell.Reveal();
        }
    }

    private void MineBoardGameOver()
    {
        for (var y = 0; y < this.currentGameOptions.Height; y++)
        {
            for (var x = 0; x < this.currentGameOptions.Width; x++)
            {
                var mineCellControl = this.mineCellControls[x, y];


                if (this.MineBoard.MineCells[x, y].HasMine)
                    mineCellControl.ShowMineImage();
                mineCellControl.Disable();
            }
        }

        this.GameOver?.Invoke();
    }
}