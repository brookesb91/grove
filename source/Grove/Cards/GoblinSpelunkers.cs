﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Dsl;

  public class GoblinSpelunkers : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return Card
        .Named("Goblin Spelunkers")
        .ManaCost("{2}{R}")
        .Type("Creature Goblin Warrior")
        .Text("{Mountainwalk}")
        .FlavorText("'It only short jump. You go first.'{EOL}'AIIIEEEE'{EOL}'Hmm . . . we go different way now.'")
        .Power(2)
        .Toughness(2)
        .Abilities(
          Static.Mountainwalk
        );
    }
  }
}