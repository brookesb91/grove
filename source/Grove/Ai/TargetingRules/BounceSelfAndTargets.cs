﻿namespace Grove.Ai.TargetingRules
{
  using System.Collections.Generic;
  using System.Linq;
  using Gameplay.Targeting;

  public class BounceSelfAndTargets : TargetingRule
  {
    protected override IEnumerable<Targets> SelectTargets(TargetingRuleParameters p)
    {
      var targetCandidates = GetCandidatesThatCanBeDestroyed(p)
        .OrderByDescending(x => x.Score)
        .ToList();

      if (targetCandidates.Count == 1 && targetCandidates[0] == p.Card)
      {
        // if owner is the only one that can be killed
        // target the owner
        return Group(targetCandidates, 1);
      }

      // otherwise return all except the owner
      return Group(targetCandidates.Where(x => x != p.Card), p.MinTargetCount());
    }
  }
}