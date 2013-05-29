﻿namespace Grove.Gameplay
{
  using System.Linq;
  using Artifical;
  using Infrastructure;
  using Messages;
  using Misc;

  public class Blocker : GameObject, IHashable
  {
    private readonly TrackableList<Damage.Damage> _assignedDamage = new TrackableList<Damage.Damage>();
    private readonly Trackable<Attacker> _attacker;
    private readonly Trackable<int> _damageAssignmentOrder = new Trackable<int>();

    private Blocker() {}

    public Blocker(Card card, Attacker attacker, Game game)
    {
      Card = card;
      Game = game;

      _attacker = new Trackable<Attacker>(attacker);
      _attacker.Initialize(ChangeTracker);

      _assignedDamage.Initialize(ChangeTracker);
      _damageAssignmentOrder.Initialize(ChangeTracker);
    }

    public Attacker Attacker { get { return _attacker.Value; } private set { _attacker.Value = value; } }
    public Card Card { get; private set; }
    public Player Controller { get { return Card.Controller; } }

    public int DamageAssignmentOrder { get { return _damageAssignmentOrder.Value; } set { _damageAssignmentOrder.Value = value; } }

    public bool HasAssignedLeathalDamage
    {
      get
      {
        return Card.HasLeathalDamage ||
          _assignedDamage.Sum(x => x.Amount) + Card.Damage >= Card.Toughness ||
            _assignedDamage.Any(x => x.IsLeathal);
      }
    }

    private bool HasAttacker { get { return Attacker != null; } }

    public int LifepointsLeft { get { return Card.Life; } }
    public int Score { get { return ScoreCalculator.CalculatePermanentScore(Card); } }
    public int DamageThisWillDealInOneDamageStep { get { return Card.CalculateCombatDamage(); } }
    public int Toughness { get { return Card.Toughness.Value; } }

    public int CalculateHash(HashCalculator calc)
    {
      return HashCalculator.Combine(
        calc.Calculate(Card),
        DamageAssignmentOrder,
        calc.Calculate(_assignedDamage));
    }

    public void AssignDamage(Damage.Damage damage)
    {
      _assignedDamage.Add(damage);
    }

    public void ClearAssignedDamage()
    {
      _assignedDamage.Clear();
    }

    public void DealAssignedDamage()
    {
      foreach (var damage in _assignedDamage)
      {
        Card.DealDamage(damage);
      }

      ClearAssignedDamage();
    }

    public void DistributeDamageToAttacker()
    {
      if (Attacker != null)
      {
        var damage = new Damage.Damage(
          source: Card,
          amount: DamageThisWillDealInOneDamageStep,
          isCombat: true,
          changeTracker: ChangeTracker
          );

        Attacker.AssignDamage(damage);
      }
    }

    public void RemoveAttacker()
    {
      Attacker = null;
    }

    public void RemoveFromCombat()
    {
      Publish(new RemovedFromCombat {Card = Card});

      if (HasAttacker)
      {
        Attacker.RemoveBlocker(this);
        Attacker = null;
      }
    }

    public bool WillBeDealtLeathalCombatDamage()
    {
      if (Attacker == null)
        return false;

      return QuickCombat.CanBlockerBeDealtLeathalCombatDamage(Card, Attacker.Card);
    }

    public bool CanKillAttacker()
    {
      if (Attacker == null)
        return false;

      return QuickCombat.CanAttackerBeDealtLeathalDamage(Attacker.Card, Card.ToEnumerable());
    }
  }
}