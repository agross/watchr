using System;

namespace Client.Demo.Fake
{
  static class Probability
  {
    static readonly Random R = new Random();

    public static bool Percent(int percent)
    {
      return R.Next(100) < percent;
    }

    public static int Next(int max)
    {
      return R.Next(max);
    }
    
    public static int Next(int min, int max)
    {
      return R.Next(min, max);
    }
  }
}