﻿namespace Grove.Core.Details.Cards.Costs
{
  using System.Linq;
  using Targeting;

  public class SacrificePermanent : Cost
  {
    public override bool CanPay(ref int? maxX)
    {
      return Controller.Battlefield.Any(
        permanent => Selector.IsValid(permanent));
    }

    public override void Pay(ITarget target, int? x)
    {
      var creature = target.Card();
      creature.Sacrifice();
    }
  }
}