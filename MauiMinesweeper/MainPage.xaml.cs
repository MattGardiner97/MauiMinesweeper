using MauiMinesweeper.Controls;
using MauiMinesweeper.Core;
using System.Runtime.CompilerServices;

namespace MauiMinesweeper;

public partial class MainPage : ContentPage
{
    private DateTime startDateTime;
    private IDispatcherTimer timer;

    public MainPage()
    {
        InitializeComponent();
    }

    private void UpdateUnmarkedMineCount()
    {
        this.UnmarkedMineCountLabel.Text = this.MineBoardControl.MineBoard.UnmarkedMineCount.ToString();
    }

    private void StartNewGame(GameOptions difficulty)
    {
        this.MineBoardControl.GameOver += this.OnGameOver;

        this.MineBoardControl.StartNewGame(difficulty);
        this.startDateTime = DateTime.Now;

        if(this.timer != null)
        {
            this.timer.Stop();
            this.timer = null;
        }

        this.timer = Dispatcher.CreateTimer();
        this.timer.Interval = TimeSpan.FromSeconds(1);
        this.timer.Tick += (_, _) =>
        {
            var elapsed = DateTime.Now - this.startDateTime;
            this.TimerLabel.Text = $"{elapsed.Minutes.ToString("D2")}:{elapsed.Seconds.ToString("D2")}";
        };
        this.timer.Start();

        UpdateUnmarkedMineCount();

        if (this.MineBoardControl.MineBoard != null)
            this.MineBoardControl.MineBoard.UnmarkedMineCountChanged -= UpdateUnmarkedMineCount;
        this.MineBoardControl.MineBoard.UnmarkedMineCountChanged += UpdateUnmarkedMineCount;
    }

    private void OnNewEasyGameClicked(object sender, EventArgs e) => StartNewGame(GameOptions.Easy);
    private void OnNewMediumGameClicked(object sender, EventArgs e) => StartNewGame(GameOptions.Medium);
    private void OnNewHardGameClicked(object sender, EventArgs e) => StartNewGame(GameOptions.Hard);

    private void OnLoaded(object sender, EventArgs e)
    {
        StartNewGame(GameOptions.Easy);
    }

    private void OnGameOver()
    {
        this.timer.Stop();
    }
}

