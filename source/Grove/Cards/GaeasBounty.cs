﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.Dsl;
  using Core.Effects;

  public class GaeasBounty : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return Card
        .Named("Gaea's Bounty")
        .ManaCost("{2}{G}")
        .Type("Sorcery")
        .Text(
          "Search your library for up to two Forest cards, reveal those cards, and put them into your hand. Then shuffle your library.")
        .FlavorText("The forest grew back so quickly that lumbering machines were suspended in the treetops.")
        .Cast(p =>
          {
            p.Timing = Timings.FirstMain();
            p.Effect = Effect<SearchLibraryPutToHand>(e =>
              {
                e.MinCount = 0;
                e.MaxCount = 2;
                e.Validator = card => card.Is("forest");
                e.Text = "Search you library for up to 2 forest cards.";
              });
          });
    }
  }
}