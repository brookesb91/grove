﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Ai;
  using Core.Effects;
  using Core.Modifiers;

  public class BlanchwoodArmor : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return C.Card
        .Named("Blanchwood Armor")
        .ManaCost("{2}{G}")
        .Type("Enchantment - Aura")
        .Text("Enchant creature{EOL}Enchanted creature gets +1/+1 for each Forest you control.")
        .FlavorText("'Before armor, there was bark. Before blades, there were thorns.'{EOL}—Molimo, maro-sorcerer")
        .Effect<EnchantCreature>((e, c) => e.Modifiers(c.Modifier<Add11ForEachForest>()))
        .Timing(Timings.Steps(Step.FirstMain))
        .Target(C.Selector(
          validator: target => target.Is().Creature,
          scorer: Core.Ai.TargetScores.YourStuffScoresMore()));

    }
  }
}