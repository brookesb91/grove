﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.Effects;
  using Core.Modifiers;

  public class VinesOfVastwood : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return C.Card
        .Named("Vines of Vastwood")
        .ManaCost("{G}")
        .KickerCost("{G}")
        .Type("Instant")
        .Timing(Any(
          Timings.ResponseToSpell(EffectCategories.LifepointReduction),
          Timings.Combat))
        .Text(
          "{Kicker} {G}{EOL}Target creature can't be the target of spells or abilities your opponents control this turn. If Vines of Vastwood was kicked, that creature gets +4/+4 until end of turn.")
        .Effect<ApplyModifiersToTarget>((e, c) => e.Modifiers(
          c.Modifier<AddStaticAbility>((m, _) => m.StaticAbility = StaticAbility.Hexproof, untilEndOfTurn: true)))
        .Target(C.Selector(
          validator: target => target.Is().Creature,
          scorer: Core.Ai.TargetScores.YourStuffScoresMore()))
        .KickerEffect<ApplyModifiersToTarget>((e, c) => e.Modifiers(
          c.Modifier<AddStaticAbility>((m, _) => m.StaticAbility = StaticAbility.Hexproof, untilEndOfTurn: true),
          c.Modifier<AddPowerAndToughness>((m, _) => {
            m.Power = 4;
            m.Toughness = 4;
          }, untilEndOfTurn: true)))
        .KickerTarget(C.Selector(
          validator: target => target.Is().Creature,
          scorer: Core.Ai.TargetScores.YourStuffScoresMore()));
    }
  }
}