using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotAllocatedPlayerByServerException : InGameException
{
    public NotAllocatedPlayerByServerException() : base("You should access the game channel via legal dedicated server. your request is denied") { }
}
