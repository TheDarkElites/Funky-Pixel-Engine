using Godot;
using System;

public abstract partial class Solid : Pixel
{
    public override void _Ready()
    {
        density = 1;
    }
}
