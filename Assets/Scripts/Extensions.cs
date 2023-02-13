// Created by Dr. Adam Streck, 2023, adam.streck@gmail.com


using System.Collections.Generic;
using System.Linq;

public static class Extensions
{
    // List shuffle extension
    public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
    {
        var rnd = new System.Random();
        return source.OrderBy(_ => rnd.Next());
    }
}