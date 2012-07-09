﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.CardDsl;
  using Core.Effects;

  public class BurstLightning : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return C.Card
        .Named("Burst Lightning")
        .ManaCost("{R}")
        .KickerCost("{4}")
        .Type("Instant")
        .Text(
          "{Kicker} {4}{EOL}Burst Lightning deals 2 damage to target creature or player. If Burst Lightning was kicked, it deals 4 damage to that creature or player instead.")
        .Timing(Timings.TargetRemovalInstant())
        .Effect<DealDamageToTarget>((e, _) => e.SetAmount(2))
        .Targets(
          filter: TargetFilters.DealDamage(2),
          selectors: C.Selector(Selectors.CreatureOrPlayer()))
        .KickerEffect<DealDamageToTarget>((e, _) => e.SetAmount(4))
        .KickerTargets(
          filter: TargetFilters.DealDamage(4),
          selectors: C.Selector(Selectors.CreatureOrPlayer()));
    }
  }
}