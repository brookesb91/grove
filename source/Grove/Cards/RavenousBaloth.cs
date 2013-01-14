﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.Costs;
  using Core.Dsl;
  using Core.Effects;
  using Core.Targeting;

  public class RavenousBaloth : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return Card
        .Named("Ravenous Baloth")
        .ManaCost("{2}{G}{G}")
        .Type("Creature - Beast")
        .Text("Sacrifice a Beast: You gain 4 life.")
        .FlavorText(
          "All we know about the Krosan Forest we have learned from those few who made it out alive.{EOL}—Elvish refugee")
        .Power(4)
        .Toughness(4)        
        .Abilities(
          ActivatedAbility(
            "Sacrifice a Beast: You gain 4 life.",
            Cost<Sacrifice>(),
            Effect<ControllerGainsLife>(e => e.Amount = 4),
            costTarget: Target(
              Validators.Card(ControlledBy.SpellOwner, card => card.Is("beast")),
              Zones.Battlefield(),
              mustBeTargetable: false),
            targetingAi: TargetingAi.CostSacrificeGainLife(),
            timing: Timings.NoRestrictions()));
    }
  }
}