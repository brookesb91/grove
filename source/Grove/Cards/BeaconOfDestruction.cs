﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.Effects;
  using Core.Zones;

  public class BeaconOfDestruction : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return C.Card
        .Named("Beacon of Destruction")
        .ManaCost("{3}{R}{R}")
        .Type("Instant")
        .Text("Beacon of Destruction deals 5 damage to target creature or player. Shuffle Beacon of Destruction into its owner's library.")
        .FlavorText("The Great Furnace's blessing is a spectacular sight, but the best view comes at a high cost.")
        .Timing(Timings.InstantRemoval)
        .Category(EffectCategories.DamageDealing)
        .AfterResolvePutToZone(Zone.Library)
        .Effect<DealDamageToTarget>((e, _) => e.Amount = 5)
        .Target(C.Selector(
          validator: target => target.IsPlayer() || target.Is().Creature,
          scorer: Core.Ai.TargetScores.OpponentStuffScoresMore(spellsDamage: 5)));

    }
  }
}