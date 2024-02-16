using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeExtensions
{
    public static int ToMilliseconds(this float seconds) => 
        (int) (seconds * 1000f);
}
