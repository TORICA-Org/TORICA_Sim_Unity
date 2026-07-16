using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AerodynamicParameters
{
    public enum GameStatus {
        Preparation,
        Flight,
        Splashdown,
        Pause,
    }
    public GameStatus Status = GameStatus.Preparation;
}
