﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.Effects;

  public class Sift : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return C.Card
        .Named("Sift")
        .ManaCost("{3}{U}")
        .Type("Sorcery")
        .Text("Draw three cards, then discard a card.")
        .FlavorText("Dwell longest on the thoughts that shine brightest.")
        .Timing(Timings.Steps(Step.FirstMain))
        .Effect<DrawCards>((e, _) =>
        {
          e.DrawCount = 3;
          e.DiscardCount = 1;
        });
    }
  }
}