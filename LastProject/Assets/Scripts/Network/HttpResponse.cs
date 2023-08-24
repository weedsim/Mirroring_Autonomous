using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class HttpResponse<Data>
{

    public int itemCount;

    public Data[] items;

}
