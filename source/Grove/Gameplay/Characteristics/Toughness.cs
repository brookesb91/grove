﻿namespace Grove.Gameplay.Characteristics
{
  using Infrastructure;
  using Modifiers;

  [Copyable]
  public class Toughness : Characteristic<int?>, IModifiable
  {
    private Toughness() {}

    public Toughness(int? value) : base(value) {}

    public void Accept(IModifier modifier)
    {
      modifier.Apply(this);
    }
  }
}