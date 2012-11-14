﻿namespace Grove.Core.Cards
{
  using Grove.Infrastructure;

  [Copyable]
  public class Attachment : IHashable
  {
    public Attachment(Card card)
    {
      Card = card;
    }

    private Attachment() {}

    public Card Card { get; private set; }

    public int CalculateHash(HashCalculator calc)
    {
      return calc.Calculate(Card);
    }
  }
}