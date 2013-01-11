﻿namespace Grove.Core.Cards.Triggers
{
  using System;
  using Infrastructure;
  using Messages;

  public class OnCastedSpell : Trigger, IReceive<PlayerHasCastASpell>
  {
    public Func<TriggeredAbility, Card, bool> Filter =
      delegate { return true; };

    public void Receive(PlayerHasCastASpell message)
    {
      if (!Filter(Ability, message.Card))
        return;

      Set(message);
    }
  }
}