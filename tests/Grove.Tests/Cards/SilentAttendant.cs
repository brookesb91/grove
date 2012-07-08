﻿namespace Grove.Tests.Cards
{
  using Core;
  using Infrastructure;
  using Xunit;

  public class SilentAttendant
  {
    public class PredefinedAi : PredefinedAiScenario
    {
      [Fact]
      public void ActivateEot()
      {
        var attendant = C("Silent Attendant");

        Battlefield(P2, attendant);

        Exec(
          At(Step.FirstMain, 4)
            .Verify(() => Equal(21, P2.Life))
          );
      }
    }
  }
}