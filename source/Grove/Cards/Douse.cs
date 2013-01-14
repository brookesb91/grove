﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.Costs;
  using Core.Dsl;
  using Core.Effects;
  using Core.Mana;
  using Core.Targeting;

  public class Douse : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return Card
        .Named("Douse")
        .ManaCost("{2}{U}")
        .Type("Enchantment")
        .Text("{1}{U}: Counter target red spell.")
        .FlavorText(
          "The academy's libraries were protected by fire-prevention spells. Even after the disaster, the books were intact—though forever sealed in time.")
        .Cast(p => p.Timing = Timings.FirstMain())
        .Abilities(
          ActivatedAbility(
            "{1}{U}: Counter target red spell.",
            Cost<PayMana>(c => c.Amount = "{1}{U}".ParseMana()),
            Effect<CounterTargetSpell>(),
            targetingAi: TargetingAi.CounterSpell(),
            effectTarget: Target(
              Validators.CounterableSpell(card => card.HasColors(ManaColors.Red)),
              Zones.Stack()),
            category: EffectCategories.Counterspell,
            timing: Timings.CounterSpell()
            )
        );
    }
  }
}