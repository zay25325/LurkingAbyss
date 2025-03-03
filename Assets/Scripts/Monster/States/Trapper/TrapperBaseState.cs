/*
File: TrapperBaseState.cs
Project: Capstone Project
Programmer: Isaiah Bartlett
First Version: 
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapperBaseState : MonsterState
{
    new protected TrapperController controller { get => base.controller as TrapperController; }
}
