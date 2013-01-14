﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Dsl;
  using Core.Effects;
  using Core.Mana;

  public class SlipperyKarst : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return Card
        .Named("Slippery Karst")
        .Type("Land")
        .Text(
          "Slippery Karst enters the battlefield tapped.{EOL}{T}: Add {G} to your mana pool.{EOL}{Cycling} {2}({2}, Discard this card: Draw a card.)")
        .Cast(p => p.Effect = Effect<PutIntoPlay>(e => e.PutIntoPlayTapped = true))
        .Cycling("{2}")
        .Abilities(
          ManaAbility(new ManaUnit(ManaColors.Green), "{T}: Add {G} to your mana pool."));
    }
  }
}