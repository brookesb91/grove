﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.Cards;
  using Core.Cards.Costs;
  using Core.Cards.Effects;
  using Core.Cards.Modifiers;
  using Core.Dsl;
  using Core.Mana;

  public class HoppingAutomaton : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return Card
        .Named("Hopping Automaton")
        .ManaCost("{3}")
        .Type("Artifact Creature")
        .Text(
          "{0}: Hopping Automaton gets -1/-1 and gains flying until end of turn.")
       .Power(2) 
       .Toughness(2)
       .Abilities(
          ActivatedAbility(
            "{0}: Hopping Automaton gets -1/-1 and gains flying until end of turn.",
            Cost<PayMana>(c => c.Amount = ManaAmount.Zero),
            Effect<ApplyModifiersToSelf>(e =>
              {
                e.ToughnessReduction = 1;
                e.Modifiers(Modifier<AddPowerAndToughness>(m =>
                  {
                    m.Power = -1;
                    m.Toughness = -1;
                  }),
                  Modifier<AddStaticAbility>(m => m.StaticAbility = Static.Flying));
              }),
            timing: All(Timings.CardHas(x => x.Toughness > 1), Timings.Steps(Step.BeginningOfCombat)))
        );
    }
  }
}