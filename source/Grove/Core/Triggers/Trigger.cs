﻿namespace Grove.Core.Triggers
{
  using System;
  using Infrastructure;

  [Copyable]
  public abstract class Trigger : IDisposable, IHashable
  {
    public event EventHandler Triggered = delegate { };

    protected TriggeredAbility Ability { get; private set; }
    protected Game Game { get; private set; }
    protected Players Players { get { return Game.Players; } }
    protected Publisher Publisher { get { return Game.Publisher; } }

    public void Dispose()
    {
      Publisher.Unsubscribe(this);
    }

    public int CalculateHash(HashCalculator calc)
    {
      return GetType().GetHashCode();
    }

    protected void Set()
    {
      Triggered(this, EventArgs.Empty);
    }

    [Copyable]
    public class Factory<TTrigger> : ITriggerFactory where TTrigger : Trigger, new()
    {
      public Initializer<TTrigger> Init = delegate { };
      public Game Game { get; set; }

      public Trigger CreateTrigger(TriggeredAbility triggeredAbility)
      {
        var trigger = new TTrigger();
        trigger.Ability = triggeredAbility;
        trigger.Game = Game;

        Init(trigger, new Creator(Game));

        Game.Publisher.Subscribe(trigger);

        return trigger;
      }
    }
  }
}