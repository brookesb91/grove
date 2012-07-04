﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.CardDsl;
  using Core.Effects;
  using Core.Modifiers;
  using Core.Triggers;
  using Core.Zones;

  public class Dread : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return C.Card
        .Named("Dread")
        .ManaCost("{3}{B}{B}{B}")
        .Type("Creature - Elemental Incarnation")
        .Text("{Fear}{EOL}Whenever a creature deals damage to you, destroy it.{EOL}When Dread is put into a graveyard from anywhere, shuffle it into its owner's library.")
        .Power(6)
        .Toughness(6)
        .Timing(Timings.Creatures())
        .Abilities(
          StaticAbility.Fear,
          C.Continuous((e, c) =>
          {
            e.ModifierFactory = c.Modifier<AddTriggeredAbility>((m, c0) =>
              m.Ability = c0.TriggeredAbility(
                "Whenever a creature deals damage to you, destroy it.",
                c0.Trigger<DealDamageToCreatureOrPlayer>((t, _) => t.ToYou()),
                c0.Effect<DestroySource>(), 
                category: EffectCategories.Destruction));
          }),
          C.TriggeredAbility(
            "When Dread is put into a graveyard from anywhere, shuffle it into its owner's library.",
            C.Trigger<ChangeZone>((t, _) => t.To = Zone.Graveyard),
            C.Effect<ShuffleIntoLibrary>())
        );
    }
  }
}