﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.Costs;
  using Core.Dsl;
  using Core.Effects;
  using Core.Mana;
  using Core.Modifiers;
  using Core.Targeting;
  using Core.Triggers;

  public class SwordOfFeastAndFamine : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return Card
        .Named("Sword of Feast and Famine")
        .ManaCost("{3}")
        .Type("Artifact - Equipment")
        .Text(
          "Equipped creature gets +2/+2 and has protection from black and from green.{EOL}Whenever equipped creature deals combat damage to a player, that player discards a card and you untap all lands you control.{EOL}{Equip} {2}")
        .Cast(p => p.Timing = Timings.FirstMain())
        .Abilities(
          TriggeredAbility(
            "Whenever equipped creature deals combat damage to a player, that player discards a card and you untap all lands you control.",
            Trigger<OnDamageDealt>(t =>
              {
                t.CombatOnly = true;
                t.UseAttachedToAsTriggerSource = true;
                t.ToPlayer();
              }),
            Effect<CompoundEffect>(e => e.ChildEffects(
              Effect<OpponentDiscardsCards>(e1 => e1.SelectedCount = 1),
              Effect<UntapAllLands>()
              ))),
          ActivatedAbility(
            "{2}: Attach to target creature you control. Equip only as a sorcery.",
            Cost<PayMana>(cost => cost.Amount = 2.Colorless()),
            Effect<Attach>(e => e.Modifiers(
              Modifier<AddPowerAndToughness>(m =>
                {
                  m.Power = 2;
                  m.Toughness = 2;
                }),
              Modifier<AddProtectionFromColors>(m => m.Colors = ManaColors.Black | ManaColors.Green))),
            effectTarget: Target(Validators.ValidEquipmentTarget(), Zones.Battlefield()),
            targetingAi: TargetingAi.CombatEquipment(),
            timing: Timings.AttachCombatEquipment(),
            activateAsSorcery: true,
            category: EffectCategories.ToughnessIncrease | EffectCategories.Protector));
    }
  }
}