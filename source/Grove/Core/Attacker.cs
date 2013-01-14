﻿namespace Grove.Core
{
  using System.Collections.Generic;
  using System.Linq;
  using Ai;
  using Decisions;
  using Decisions.Results;
  using Infrastructure;
  using Messages;

  [Copyable]
  public class Attacker : IHashable
  {
    private readonly TrackableList<Damage> _assignedDamage;
    private readonly TrackableList<Blocker> _blockers;
    private readonly Card _card;
    private readonly Game _game;
    private readonly Trackable<bool> _isBlocked;

    public Attacker(Card card, Game game)
    {
      _card = card;
      _game = game;
      _blockers = new TrackableList<Blocker>(game.ChangeTracker);
      _assignedDamage = new TrackableList<Damage>(game.ChangeTracker);
      _isBlocked = new Trackable<bool>(game.ChangeTracker);
    }

    private Attacker() {}

    public IEnumerable<Blocker> Blockers { get { return _blockers; } }
    public int BlockersCount { get { return _blockers.Count; } }
    public IEnumerable<Blocker> BlockersInDamageAssignmentOrder { get { return _blockers.OrderBy(x => x.DamageAssignmentOrder); } }
    public Card Card { get { return _card; } }
    public Player Controller { get { return _card.Controller; } }
    public bool HasDeathTouch { get { return _card.Has().Deathtouch; } }
    public bool HasTrample { get { return _card.Has().Trample; } }
    public int LifepointsLeft { get { return _card.Life; } }
    public int DamageThisWillDealInOneDamageStep { get { return _card.CalculateCombatDamage(); } }

    public int CalculateHash(HashCalculator calc)
    {
      return HashCalculator.Combine(
        calc.Calculate(_isBlocked),
        calc.Calculate(_card),
        calc.Calculate(_blockers),
        calc.Calculate(_assignedDamage));
    }

    public void AddBlocker(Blocker blocker)
    {
      _blockers.Add(blocker);
      _isBlocked.Value = true;
    }

    public void AssignDamage(Damage damage)
    {
      _assignedDamage.Add(damage);
    }

    public bool CanBeBlockedBy(Card creature)
    {
      return _card.CanBeBlockedBy(creature);
    }

    public void DealAssignedDamage()
    {
      foreach (Damage damage in _assignedDamage)
      {
        _card.DealDamage(damage);
      }

      _assignedDamage.Clear();
    }

    public void DistributeDamageToBlockers()
    {
      _game.Enqueue<AssignCombatDamage>(
        controller: _card.Controller,
        init: p => p.Attacker = this);
    }

    public void DistributeDamageToBlockers(DamageDistribution distribution)
    {
      foreach (Blocker blocker in _blockers)
      {
        var damage = new Damage(
          source: Card,
          amount: distribution[blocker],
          isCombat: true,
          changeTracker: _game.ChangeTracker
          );

        blocker.AssignDamage(damage);
      }

      Player defender = _game.Players.GetOpponent(_card.Controller);

      if (HasTrample || _isBlocked == false)
      {
        var unassignedDamage = new Damage(
          source: _card,
          amount: DamageThisWillDealInOneDamageStep - distribution.Total,
          isCombat: true,
          changeTracker: _game.ChangeTracker);

        defender.AssignDamage(unassignedDamage);
      }
    }

    public int GetDamageThisWillDealToPlayer()
    {
      if (_blockers.Count == 0)
        return _card.CalculateCombatDamage(allDamageSteps: true);

      if (HasTrample)
      {
        return QuickCombat.CalculateTrampleDamage(Card, _blockers.Select(x => x.Card));
      }

      return 0;
    }

    public bool HasBlocker(Blocker blocker)
    {
      return _blockers.Contains(blocker);
    }

    public void RemoveBlocker(Blocker blocker)
    {
      _blockers.Remove(blocker);
    }

    public void RemoveFromCombat()
    {
      _game.Publish(new RemovedFromCombat {Card = Card});

      foreach (Blocker blocker in _blockers)
      {
        blocker.RemoveAttacker();
      }
    }

    public void SetDamageAssignmentOrder(DamageAssignmentOrder damageAssignmentOrder)
    {
      foreach (Blocker blocker in _blockers)
      {
        blocker.DamageAssignmentOrder = damageAssignmentOrder[blocker];
      }
    }

    public static implicit operator Card(Attacker attacker)
    {
      return attacker != null ? attacker._card : null;
    }

    public bool CanBeDealtLeathalCombatDamage()
    {
      return QuickCombat.CanAttackerBeDealtLeathalDamage(Card, _blockers.Select(x => x.Card));
    }

    public bool CanKillAnyBlocker()
    {
      return QuickCombat.CanAttackerKillAnyBlocker(Card, _blockers.Select(x => x.Card));
    }
  }
}