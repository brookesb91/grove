﻿namespace Grove.Core.Ai
{
  using System;
  using System.Collections.Generic;
  using System.Linq;

  public static class ScoreCalculator
  {
    private static readonly Dictionary<int, int> LifeToScore = new Dictionary<int, int>{
      {20, 0},
      {19, -40},
      {18, -80},
      {17, -120},
      {16, -160},
      {15, -200},
      {14, -250},
      {13, -300},
      {12, -350},
      {11, -420},
      {10, -490},
      {9, -560},
      {8, -650},
      {7, -740},
      {6, -830},
      {5, -920},
      {4, -1120},
      {3, -1220},
      {2, -1350},
      {1, -1500},
    };

    private static readonly Dictionary<int, int> ManaCostToScore = new Dictionary<int, int>{
      {1, 150},
      {2, 250},
      {3, 320},
      {4, 390},
      {5, 440},
      {6, 490},
      {7, 540},
    };

    private static readonly Dictionary<int, int> PowerToughnessToScore = new Dictionary<int, int>{
      {0, 0},
      {1, 50},
      {2, 100},
      {3, 150},
      {4, 200},
      {5, 250},
      {6, 300},
      {7, 350},
      {8, 400},
      {9, 450},
      {10, 500},
    };

    public static int CalculateDiscardScore(Card card)
    {
      // the lower the score, the more likely card will be discarded      
      var hand = card.Controller.Hand;
      var battlefield = card.Controller.Battlefield;

      var sameCardBattleFieldCount = battlefield.Count(x => x.Name == card.Name);

      if (card.Is().Land)
      {
        var landHandCount = hand.Count(x => x.Is().Land);
        var landBattlefieldCount = battlefield.Count(x => x.Is().Land);

        if ((landHandCount + landBattlefieldCount) < 4)
          return int.MaxValue;

        if (sameCardBattleFieldCount < 2)
          return int.MaxValue;

        if (landHandCount < 2)
          return int.MaxValue;

        return 0 - 2*sameCardBattleFieldCount;
      }

      return 0 - card.ManaCost.Count();
    }

    public static int CalculateLifeScore(int life)
    {
      int score = 5000;
                              
      if (life > 20)
        return score + (life - 20)*10;

      if (life <= 0)
        return 0;

      return score + LifeToScore[life];
    }
    
    public static int CalculatePermanentScore(Card permanent)
    {
      const int landValue = 150;      
      const int tappedPermanentValue = -1;
      var score = 0;

      if (permanent.IsTapped)
        score += tappedPermanentValue;

      if (permanent.ManaCost != null)
      {
        score += CalculatePermanentScoreFromManaCost(permanent.ManaCost);
      }
      else if (permanent.Is().Creature)
      {
        score += CalculatePermanentScoreFromPowerToughness(permanent.Power.Value, permanent.Toughness.Value);
      }      
      else if (permanent.Is().Land)
      {
        score += landValue;

        if (!permanent.Is().BasicLand)
          score += 10;
      }

      return score;
    }    

    private static int CalculatePermanentScoreFromManaCost(IManaAmount mana)
    {
      var converted = Math.Min(7, mana.Converted);            
      return ManaCostToScore[converted];
    }

    private static int CalculatePermanentScoreFromPowerToughness(int power, int toughness)
    {
      var powerToughness = power + toughness;

      if (powerToughness < 0)
        powerToughness = 0;
      else if (powerToughness > 10)
        powerToughness = 10;           
      
      return PowerToughnessToScore[powerToughness];
    }

    public static int CalculateCardInHandScore(Card card)
    {
      return 140;
    }

    public static int CalculateLifelossScore(int life, int loss)
    {
      return CalculateLifeScore(life) - CalculateLifeScore(life - loss);
    }
  }
}