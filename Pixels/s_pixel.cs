using Godot;
using System;

public partial class s_pixel : pixel
{
    public override void _Ready()
    {
        base._Ready();
        doUpdate = false;
        isStatic = true;

        Name = "StaticPixel";
    }
}