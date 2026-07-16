using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameParameters
{
    public enum Status {
        Preparation,
        Flight,
        Splashdown,
        Pause,
    }
    public Status status = Status.Preparation;
}
