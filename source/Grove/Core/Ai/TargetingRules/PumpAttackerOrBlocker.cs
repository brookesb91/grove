﻿namespace Grove.Core.Ai.TargetingRules
{
  using System.Collections.Generic;
  using Targeting;

  public class PumpAttackerOrBlocker : TargetingRule
  {
    public int? Power;
    public int? Toughness;

    protected override IEnumerable<Targets> SelectTargets(TargetingRuleParameters p)
    {
      var power = Power ?? p.MaxX;
      var toughness = Toughness ?? p.MaxX;

      var candidates = None<Card>();

      if (p.Controller.IsActive && Turn.Step == Step.DeclareBlockers)
      {
        candidates = GetCandidatesForAttackerPowerToughnessIncrease(power, toughness, p);
      }

      else if (!p.Controller.IsActive && Turn.Step == Step.DeclareBlockers)
      {
        candidates = GetCandidatesForBlockerPowerToughnessIncrease(power, toughness, p);
      }

      return Group(candidates, p.MinTargetCount());
    }
  }
}