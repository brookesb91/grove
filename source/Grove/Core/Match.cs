﻿namespace Grove.Core
{
  using System.Threading.Tasks;
  using System.Windows;
  using Ui.GameResults;
  using Ui.Shell;

  public class Match
  {
    public const string Player1Name = "You";
    public const string Player2Name = "Hal";

    private readonly Game.IFactory _gameFactory;
    private readonly ViewModel.IFactory _gameResultsFactory;
    private readonly Ui.MatchResults.ViewModel.IFactory _matchResultsFactory;
    private readonly Ui.PlayScreen.ViewModel.IFactory _playScreenFactory;
    private readonly Player.IFactory _playerFactory;
    private readonly Ui.StartScreen.ViewModel.IFactory _startScreenFactory;
    private readonly TaskScheduler _uiScheduler;
    private string _deck1;
    private string _deck2;
    private int? _looser;
    private bool _playerLeftMatch;
    private bool _rematch = true;
    private bool _shutdown;

    public Match(
      Ui.PlayScreen.ViewModel.IFactory playScreenFactory,
      Ui.StartScreen.ViewModel.IFactory startScreenFactory,
      ViewModel.IFactory gameResultsFactory,
      Ui.MatchResults.ViewModel.IFactory matchResultsFactory,
      Game.IFactory gameFactory,
      Player.IFactory playerFactory)
    {
      _gameFactory = gameFactory;
      _playerFactory = playerFactory;
      _playScreenFactory = playScreenFactory;
      _startScreenFactory = startScreenFactory;
      _gameResultsFactory = gameResultsFactory;
      _matchResultsFactory = matchResultsFactory;
      _uiScheduler = TaskScheduler.FromCurrentSynchronizationContext();

      Application.Current.Exit += delegate { _shutdown = true; };
    }

    public Game Game { get; private set; }

    public bool IsFinished
    {
      get
      {
        return
          Player1WinCount == 2 ||
          Player2WinCount == 2;
      }
    }

    public int Player1WinCount { get; private set; }
    public int Player2WinCount { get; private set; }
    public IShell Shell { get; set; }

    protected Player Looser
    {
      get
      {
        if (_looser == null)
          return null;

        return Game.Players[_looser.Value];
      }
    }

    public void Start(string player1Deck, string player2Deck)
    {
      _deck1 = player1Deck + ".dec";
      _deck2 = player2Deck + ".dec";

      ResetResults();
      Run();
    }

    private void DisplayGameResults()
    {
      var viewModel = _gameResultsFactory.Create();
      Shell.ShowModalDialog(viewModel);
      _playerLeftMatch = viewModel.PlayerLeftMatch;
    }

    private void DisplayMatchResults()
    {
      var viewModel = _matchResultsFactory.Create();
      Shell.ShowModalDialog(viewModel);
      _rematch = viewModel.ShouldRematch;
    }

    private void ResetResults()
    {
      Player1WinCount = 0;
      Player2WinCount = 0;
      _playerLeftMatch = false;
      _looser = null;
    }

    private void Run()
    {
      Bootstrapper.NewGame();

      var backgroundTask =
        new Task(() =>
          {
            Game = _gameFactory.Create();

            Game.Players.Player1 = _playerFactory.Create(
              name: Player1Name,
              avatar: "player1.png",
              isHuman: true,
              deck: _deck1
              );

            Game.Players.Player2 = _playerFactory.Create(
              name: Player2Name,
              avatar: "player2.png",
              isHuman: false,
              deck: _deck2
              );


            var playScreen = _playScreenFactory.Create();
            Shell.ChangeScreen(playScreen);

            Game.Start(looser: Looser);

            var looser = UpdateScore();
            SetLooser(looser);
          });

      backgroundTask.ContinueWith(task =>
        {
          if (IsFinished)
          {
            DisplayMatchResults();

            if (_shutdown)
            {
              return;
            }

            if (_rematch && !_playerLeftMatch)
            {
              Start(_deck1, _deck2);
              return;
            }

            ShowStartScreen();
            return;
          }

          DisplayGameResults();

          if (_shutdown)
          {
            return;
          }

          if (_playerLeftMatch)
          {
            ShowStartScreen();
            return;
          }

          Run();
        }, _uiScheduler);

      backgroundTask.Start(TaskScheduler.Default);
    }

    private void SetLooser(int? looser)
    {
      _looser = looser;
    }

    private void ShowStartScreen()
    {
      var startScreen = _startScreenFactory.Create();
      Shell.ChangeScreen(startScreen);
    }

    private int? UpdateScore()
    {
      if (Game.Players.BothHaveLost)
        return null;

      if (Game.Players.Player1.HasLost)
      {
        Player2WinCount++;
        return 0;
      }

      Player1WinCount++;
      return 1;
    }
  }
}