﻿namespace Grove.Cards
{
  using System.Collections.Generic;
  using Core;
  using Core.Dsl;
  using Core.Effects;
  using Core.Triggers;

  public class EndlessWurm : CardsSource
  {
    public override IEnumerable<ICardFactory> GetCards()
    {
      yield return Card
        .Named("Endless Wurm")
        .ManaCost("{3}{G}{G}")
        .Type("Creature Wurm")
        .Text(
          "{Trample}{EOL}At the beginning of your upkeep, sacrifice Endless Wurm unless you sacrifice an enchantment.")
        .FlavorText("Ages ago, a party of elves took cover to let one pass. They're still waiting.")
        .Power(9)
        .Toughness(9)
        .Abilities(
          Static.Trample,
          TriggeredAbility(
            "At the beginning of your upkeep, sacrifice Endless Wurm unless you sacrifice an enchantment.",
            Trigger<OnStepStart>(t => { t.Step = Step.Upkeep; }),
            Effect<SacPermanentOrSacrificeOwner>(e =>
              {
                e.ShouldPayAi = (controller, owner) => owner.IsAbleToAttack;
                e.Validator = card => card.Is().Enchantment;
                e.Text = "Select enchantment to sacrifice";
              }),
            triggerOnlyIfOwningCardIsInPlay: true));
    }
  }
}