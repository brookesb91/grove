﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.CardDsl;
  using Core.Effects;

  public class Shock : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return C.Card
        .Named("Shock")
        .ManaCost("{R}")
        .Type("Instant")
        .Timing(Timings.TargetRemovalInstant())
        .Text("Shock deals 2 damage to target creature or player.")
        .Effect<DealDamageToTarget>((e, _) => e.Amount = 2)
        .Targets(
          filter: TargetFilters.DealDamage(2),
          selectors: C.Selector(Selectors.CreatureOrPlayer()));
    }
  }
}