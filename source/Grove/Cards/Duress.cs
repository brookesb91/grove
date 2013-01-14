﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.Dsl;
  using Core.Effects;

  public class Duress : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return Card
        .Named("Duress")
        .ManaCost("{B}")
        .Type("Sorcery")
        .Text(
          "Target opponent reveals his or her hand. You choose a noncreature, nonland card from it. That player discards that card.")
        .FlavorText("'We decide who is worthy of our works.'{EOL}—Gix, Yawgmoth praetor")
        .Cast(p =>
          {
            p.Timing = All(Timings.FirstMain(), Timings.OpponentHasCardsInHand(1));
            p.Effect = Effect<OpponentDiscardsCards>(e =>
              {
                e.SelectedCount = 1;
                e.Filter = card => !card.Is().Creature && !card.Is().Land;
                e.YouChooseDiscardedCards = true;
              });
          });
    }
  }
}