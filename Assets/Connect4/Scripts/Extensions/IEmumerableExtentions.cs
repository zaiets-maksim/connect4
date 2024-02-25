using System;
using System.Collections.Generic;
using System.Linq;

namespace Connect4.Scripts.Extensions
{
  public static class IEmumerableExtentions
  {
    public static T Random<T>(this IEnumerable<T> enumerable)
    {
      var items = enumerable.ToArray();
      
      if (items.Length == 0)
        throw new InvalidOperationException("Sequence is empty");

      return items[UnityEngine.Random.Range(0, items.Length)];
    }
  }
}