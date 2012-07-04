﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.CardDsl;

  public class BaneslayerAngel : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return C.Card
        .Named("Baneslayer Angel")
        .ManaCost("{3}{W}{W}")
        .Type("Creature - Angel")
        .Text("{Flying}, {First strike}, {Lifelink}{EOL}Baneslayer Angel has protection from Demons and from Dragons.")
        .FlavorText("Some angels protect the meek and innocent. Others seek out and smite evil wherever it lurks.")
        .Power(5)
        .Toughness(5)
        .Timing(Timings.Creatures())
        .Protections("demon", "dragon")
        .Abilities(
          StaticAbility.Flying,
          StaticAbility.FirstStrike,
          StaticAbility.Lifelink
        );
    }
  }
}