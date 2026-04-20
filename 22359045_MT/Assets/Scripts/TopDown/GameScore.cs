using System;
using UnityEngine;

public static class GameScore
{
    public static int Current { get; private set; }
    public static event Action<int> Changed;

    public static void ResetScore()
    {
        Current = 0;
        Changed?.Invoke(Current);
    }

    public static void Add(int v)
    {
        Current += v;
        Changed?.Invoke(Current);
    }
}
