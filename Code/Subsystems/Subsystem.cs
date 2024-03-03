using Godot;
using System.Collections.Generic;
public abstract partial class Subsystem : GodotObject
{
    public abstract void Fire(Stack<Pixel> UpdateStack);
}