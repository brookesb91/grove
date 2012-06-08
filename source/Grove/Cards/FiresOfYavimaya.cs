﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.Costs;
  using Core.Effects;
  using Core.Modifiers;

  public class FiresOfYavimaya : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return C.Card
        .Named("Fires of Yavimaya")
        .ManaCost("{1}{R}{G}")
        .Type("Enchantment")
        .Text(
          "Creatures you control have haste.{EOL}Sacrifice Fires of Yavimaya: Target creature gets +2/+2 until end of turn.")
        .Timing(Timings.Steps(Step.FirstMain))
        .Abilities(
          C.Continuous((e, c) =>
          {
            e.ModifierFactory = c.Modifier<AddStaticAbility>(
              (m, _) => m.StaticAbility = StaticAbility.Haste);
            e.Filter = (card, source) => card.Controller == source.Controller;
          }),
          C.ActivatedAbility(
            "Sacrifice Fires of Yavimaya: Target creature gets +2/+2 until end of turn.",
            C.Cost<SacrificeOwner>(),
            C.Effect<ApplyModifiersToTarget>((e, c) => e.Modifiers(
              c.Modifier<AddPowerAndToughness>((m, _) =>
              {
                m.Power = 2;
                m.Toughness = 2;
              }, untilEndOfTurn: true))),
            C.Selector(
              validator: target => target.Is().Creature,
              scorer: Core.Ai.TargetScores.YourStuffScoresMore()),
            timing: Any(
              Timings.ResponseToSpell(EffectCategories.PwTReduction),
              Timings.Combat))
        );
    }
  }
}