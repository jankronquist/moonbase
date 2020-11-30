using System;
using System.Collections.Generic;

public static class RandomUtil {

    public static T Pick<T>(T[] ts) {
        return ts[UnityEngine.Random.Range(0, ts.Length)];
    }

    public static List<T> Shuffle<T>(List<T> list) {
        System.Random _random = new System.Random();

        int n = list.Count;
        for (int i = 0; i < n; i++) {
            int r = i + (int)(_random.NextDouble() * (n - i));
            T replace = list[r];
            list[r] = list[i];
            list[i] = replace;
        }

        return list;
    }
}
