﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.Costs;
  using Core.Dsl;
  using Core.Mana;
  using Core.Modifiers;
  using Core.Targeting;

  public class VinesOfVastwood : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return Card
        .Named("Vines of Vastwood")
        .ManaCost("{G}")
        .Type("Instant")
        .Text(
          "{Kicker} {G}{EOL}Target creature can't be the target of spells or abilities your opponents control this turn. If Vines of Vastwood was kicked, that creature gets +4/+4 until end of turn.")
        .Cast(p =>
          {
            p.Category = EffectCategories.Protector;
            p.Effect = Effect<Core.Effects.ApplyModifiersToTargets>(e => e.Modifiers(
              Modifier<AddStaticAbility>(m => m.StaticAbility = Static.Hexproof, untilEndOfTurn: true)));
            p.EffectTargets = L(Target(Validators.Card(x => x.Is().Creature), Zones.Battlefield()));
            p.TargetingAi = TargetingAi.ShieldHexproof();
          })
        .Cast(p =>
          {
            p.Description = p.KickerDescription;
            p.Cost = Cost<PayMana>(c => c.Amount = "{G}{G}".ParseMana());
            p.Category = EffectCategories.Protector | EffectCategories.ToughnessIncrease;
            p.Effect = Effect<Core.Effects.ApplyModifiersToTargets>(e => e.Modifiers(
              Modifier<AddStaticAbility>(m => m.StaticAbility = Static.Hexproof, untilEndOfTurn: true),
              Modifier<AddPowerAndToughness>(m =>
                {
                  m.Power = 4;
                  m.Toughness = 4;
                }, untilEndOfTurn: true)));
            p.EffectTargets = L(Target(Validators.Card(x => x.Is().Creature), Zones.Battlefield()));
            p.TargetingAi = Any(TargetingAi.ShieldHexproof(), TargetingAi.IncreasePowerAndToughness(4, 4));
          });
    }
  }
}