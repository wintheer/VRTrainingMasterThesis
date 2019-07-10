using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Operation
{
    public string operationName;
    // Start is called before the first frame update
    public Operation(string name){
        operationName = name;
    }
    public string getName(){
        return operationName;
    }
    public void setName(string oName) {
        operationName = oName;
    }
}
