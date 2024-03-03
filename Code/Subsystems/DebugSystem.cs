
using System.Collections.Generic;
using System;
using Godot;

public partial class DebugSystem : Subsystem
{
    //Tracks updateQueue mask
    private List<Sprite2D> updateMaskList;
    //Last mouse pos for pixel spawning debug
    private Vector2 lastMousePos;
    public DebugSystem()
    {
        updateMaskList = new List<Sprite2D>();
    }
    public override void Fire(Stack<Pixel> UpdateStack)
    {
        if(GMS.GM.GetNode<CheckBox>("/root/Main/Control/ShowUpdateMask").ButtonPressed)
        {
            UpdateMaskClear();
            
            Stack<Pixel> updateTexturePixelStack = new Stack<Pixel>(UpdateStack);

            while(updateTexturePixelStack.Count > 0)
            {
                Pixel currentPixel = updateTexturePixelStack.Pop();
                if(!IsInstanceValid(currentPixel))
                {
                    continue;
                }
                Texture2D debugTexture = GD.Load<Texture2D>("res://Textures/DebugRed.png");
                Sprite2D newMask = new Sprite2D() {Texture = debugTexture, Scale = new Vector2(2, 2), Position = currentPixel.Position};
                updateMaskList.Add(newMask);
                GMS.GM.AddChild(newMask);
            }
        }

        if(UpdateStack.Count > 0 || (GMS.GM.GetViewport().GetMousePosition() != lastMousePos))
		{
			HandlePixelTools();
            lastMousePos = GMS.GM.GetViewport().GetMousePosition();
		}
    }

    public void UpdateMaskClear()
    {   
        foreach(Sprite2D mask in updateMaskList)
        {
            mask.QueueFree();
        }

        updateMaskList.Clear();
    }

    public void HandlePixelTools()
	{
		//The following handles placing pixels
		if((Input.IsMouseButtonPressed(MouseButton.Left) || Input.IsMouseButtonPressed(MouseButton.Right)) && !Input.IsKeyPressed(Key.Ctrl))
		{	
			string pixelID;
			switch(GMS.GM.GetNode<OptionButton>("/root/Main/Control/PixelSelector").Selected)
			{
				case 0:
					pixelID = "res://Pixels/dirt.tscn";
					break;
				case 1:
					pixelID = "res://Pixels/s_pixel.tscn";
					break;
                case 2:
                    pixelID = "res://Pixels/sand.tscn";
					break;
				case 3:
					pixelID = "res://Pixels/water.tscn";
					break;
				case 4:
					pixelID = "res://Pixels/steam.tscn";
					break;
				default:
					pixelID = "res://Pixels/s_pixel.tscn";
					break;
			}
			
			Vector2 mousePosition = GMS.GM.GetViewport().GetMousePosition();
			int blobSize = Int32.Parse(GMS.GM.GetNode<LineEdit>("/root/Main/Control/SizeSetter").Text);
			
			switch(blobSize)
			{
				case 0:
					break;
				case 1:
					if(Input.IsMouseButtonPressed(MouseButton.Left))
					{
						Pixel newPix = PixelLib.CreatePixel(pixelID, mousePosition);
						if(newPix == null)
						{
							GD.PushError(String.Format("Newly created Pixel was null!"));
							break;
						}
						GMS.GM.AddChild(newPix);
					}
					else if(Input.IsMouseButtonPressed(MouseButton.Right))
					{
                        Vector2 delPos = PixelLib.GetPixelCoordinates(mousePosition);
                        if(GMS.GM.pixelDict.ContainsKey(delPos))
                            {
                                PixelLib.DeletePixel(GMS.GM.pixelDict[delPos]);
                            }
					}
					break;
				default:
					for(int x = -blobSize/2; x < blobSize/2; x++)
					{
						for(int y = -blobSize/2; y < blobSize/2; y++)
						{
							if(Input.IsMouseButtonPressed(MouseButton.Left))
							{
								Pixel tempPix = PixelLib.CreatePixel(pixelID, new Vector2(mousePosition.X + (x * GLOB.PIXSIZE), mousePosition.Y + (y * GLOB.PIXSIZE)));
								if(tempPix == null)
								{
									//GD.PushError(String.Format("Newly created Pixel was null!"));
									continue;
								}
								GMS.GM.AddChild(tempPix);
							}
							else if(Input.IsMouseButtonPressed(MouseButton.Right))
							{
								Vector2 delPos = PixelLib.GetPixelCoordinates(new Vector2(mousePosition.X + (x * GLOB.PIXSIZE), mousePosition.Y + (y * GLOB.PIXSIZE)));
								if(GMS.GM.pixelDict.ContainsKey(delPos))
									{
										PixelLib.DeletePixel(GMS.GM.pixelDict[delPos]);
									}
							}
						}
					}
					break;
			}
		}
	}
}