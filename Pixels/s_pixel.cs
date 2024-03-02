using Godot;
using System;

public partial class s_pixel : Pixel
{
    public override void _Ready()
    {
        base._Ready();
        isStatic = true;

        Name = "StaticPixel";
    }
}
