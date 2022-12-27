namespace MauiMinesweeper.Controls;

using MauiMinesweeper.Core;

public partial class MineCellControl : ContentView
{
    internal MineCell MineCell { get; private set; }

    internal Action<MineCellControl> DoubleTapped;

    public MineCellControl()
    {
        InitializeComponent();
    }

    internal void Reset(MineCell mineCell)
    {
        this.MineCell = mineCell;
        mineCell.Revealed += this.OnRevealed;
        mineCell.Marked += this.OnMarked;
        mineCell.Unmarked += this.OnUnmarked;

        this.IsEnabled = true;
        this.MainGrid.IsEnabled = true;
        this.MineImage.IsVisible = false;

        var primaryGestureRecognizer = new TapGestureRecognizer() { NumberOfTapsRequired = 1, Buttons = ButtonsMask.Primary, };
        primaryGestureRecognizer.Tapped += OnPrimaryTapped;

        var primaryDoubleGestureRecognizer = new TapGestureRecognizer() { NumberOfTapsRequired = 1, Buttons = ButtonsMask.Primary, };
        primaryGestureRecognizer.Tapped += OnPrimaryDoubleTapped;

        var secondaryGestureRecognizer = new TapGestureRecognizer() { NumberOfTapsRequired = 1, Buttons = ButtonsMask.Secondary };
        secondaryGestureRecognizer.Tapped += OnSecondaryTapped;

        this.GestureRecognizers.Clear();
        this.GestureRecognizers.Add(primaryGestureRecognizer);
        this.GestureRecognizers.Add(primaryDoubleGestureRecognizer);
        this.GestureRecognizers.Add(secondaryGestureRecognizer);

        VisualStateManager.GoToState(this.MainGrid, "Normal");
    }

    internal void ShowMineImage() => this.MineImage.IsVisible = true;

    internal void Disable()
    {
        this.IsEnabled = false;
        this.MainGrid.IsEnabled = false;
        this.GestureRecognizers.Clear();
    }

    private void OnPrimaryTapped(object sender, EventArgs e)
    {
        this.MineCell.Reveal();
    }

    private void OnPrimaryDoubleTapped(object sender, EventArgs e)
    {
        this.DoubleTapped?.Invoke(this);
    }

    private void OnSecondaryTapped(object sender, EventArgs e)
    {
        if (!this.MineCell.IsMarked)
            this.MineCell.Mark();
        else
            this.MineCell.Unmark();
    }

    private void OnRevealed(MineCell _)
    {
        this.MainLabel.Text = this.MineCell.AdjacentMineCount == 0 ? string.Empty : this.MineCell.AdjacentMineCount.ToString();
        if (this.MineCell.HasMine)
        {
            VisualStateManager.GoToState(this.MainGrid, "MineRevealed");
            this.ShowMineImage();
        }
        else
        {
            VisualStateManager.GoToState(this.MainGrid, "Revealed");
        }

        VisualStateManager.GetVisualStateGroups(this.MainGrid)[0].States.Clear();
    }

    private void OnMarked(MineCell _)
    {
        this.FlagImage.IsVisible = true;
    }

    private void OnUnmarked(MineCell _)
    {
        this.FlagImage.IsVisible = false;
    }
}