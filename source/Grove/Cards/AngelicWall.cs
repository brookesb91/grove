﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.CardDsl;

  public class AngelicWall : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return C.Card
        .Named("Angelic Wall")
        .ManaCost("{1}{W}")
        .Type("Creature Wall")
        .Text("{Defender}, {Flying}")
        .FlavorText(
          "'The air stirred as if fanned by angels wings, and the enemy was turned aside.'{EOL}—Tales of Ikarov the Voyager")
        .Power(0)
        .Toughness(4)
        .Timing(Timings.Creatures())
        .Abilities(
          StaticAbility.Defender,
          StaticAbility.Flying
        );
    }
  }
}