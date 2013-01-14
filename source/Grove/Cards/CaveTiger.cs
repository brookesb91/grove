﻿namespace Grove.Cards
{
  using System;
  using System.Collections.Generic;
  using Core;
  using Core.Dsl;
  using Core.Effects;
  using Core.Modifiers;
  using Core.Triggers;

  public class CaveTiger : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return Card
        .Named("Cave Tiger")
        .ManaCost("{2}{G}")
        .Type("Creature Cat")
        .Text("Whenever Cave Tiger becomes blocked by a creature, Cave Tiger gets +1/+1 until end of turn.")
        .FlavorText(
          "The druids found a haven in the cool limestone tunnels beneath Argoth. The invaders found only tigers.")
        .Power(2)
        .Toughness(2)
        .Abilities(
          TriggeredAbility(
            "Whenever Cave Tiger becomes blocked by a creature, Cave Tiger gets +1/+1 until end of turn.",
            Trigger<OnBlock>(t => t.GetsBlocked = true),
            Effect<ApplyModifiersToSelf>(e => e.Modifiers(
              Modifier<AddPowerAndToughness>(m =>
                {
                  m.Power = 1;
                  m.Toughness = 1;
                }, untilEndOfTurn: true))),
            triggerOnlyIfOwningCardIsInPlay: true)
        );
    }
  }
}