using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Params
{
    public enum GameStatus {
        Preparation,
        Flight,
        Splashdown,
        Pause,
    }
    public static GameStatus Status = GameStatus.Preparation;
}
