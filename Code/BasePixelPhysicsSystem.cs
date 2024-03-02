using Godot;
using System;
using System.Collections.Generic;

public partial class BasePixelPhysicsSystem : GodotObject
{	
    private PixelLib pixelLib;

    private Vector2 lastMousePos;

    public BasePixelPhysicsSystem()
    {
        pixelLib = new PixelLib();
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame. Returns a list of pixels to update
	public void RunSim(Stack<Pixel> UpdateStack)
	{
		//Begin Pixel simulation
		bool pixelsUpdated = false;

        Random rand = new Random(); 

		while(UpdateStack.Count > 0)
		{
			Pixel currentPixel = UpdateStack.Pop();
            bool pixelUpdated = false;
            
			if(!IsInstanceValid(currentPixel))
			{
				continue;
			}

            //Meta Calulations
			if(currentPixel.Position.Y > 600 || currentPixel.Position.Y <= 0)
			{
                pixelLib.DeletePixel(currentPixel);
				pixelsUpdated = true;
                pixelUpdated = true;
			}

            if(currentPixel.isStatic) //Dont need to update static pixels or pixels not scheduled to update
            {
				continue;
            }
			
            //Gravity Calculations
            //Amount of required connections to pixels to avoid us from falling
			int reqConnections = (int)Math.Ceiling(8 - Math.Clamp(currentPixel.ductility * 16, 0, 4));
            if(pixelLib.GetPixelsInRange(currentPixel.Position, currentPixel.GetType(), includeCorners: true).Length < reqConnections)
            {
                if(currentPixel is Solid || currentPixel is Liquid)
				{
					if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X, currentPixel.Position.Y + GLOB.PIXSIZE), currentPixel))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X, currentPixel.Position.Y + GLOB.PIXSIZE), currentPixel);
						pixelsUpdated = true;
						pixelUpdated = true;
					} 
					else if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y + GLOB.PIXSIZE), currentPixel) && (rand.Next(1,101) <= (100 * (1 - currentPixel.ductility))))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y + GLOB.PIXSIZE), currentPixel);
						pixelsUpdated = true;
						pixelUpdated = true;
					}
					else if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y + GLOB.PIXSIZE), currentPixel) && (rand.Next(1,101) <= (100 * (1 - currentPixel.ductility))))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y + GLOB.PIXSIZE), currentPixel);
						pixelsUpdated = true;
						pixelUpdated = true;
					}
				}

				if(currentPixel is Liquid && !pixelUpdated)
				{
					if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel) && (rand.NextDouble() >= 0.5 || !pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel)))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel);
						pixelsUpdated = true;
						pixelUpdated = true;
					}
					else if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel);
						pixelsUpdated = true;
						pixelUpdated = true;
					}
				}

				if(currentPixel is Gas)
				{
					if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X, currentPixel.Position.Y - GLOB.PIXSIZE), currentPixel))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X, currentPixel.Position.Y - GLOB.PIXSIZE), currentPixel);
						pixelsUpdated = true;
						pixelUpdated = true;
					} 
					else if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y - GLOB.PIXSIZE), currentPixel) && (rand.Next(1,101) <= (100 * (1 - currentPixel.ductility))))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y - GLOB.PIXSIZE), currentPixel);
						pixelsUpdated = true;
						pixelUpdated = true;
					}
					else if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y - GLOB.PIXSIZE), currentPixel) && (rand.Next(1,101) <= (100 * (1 - currentPixel.ductility))))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y - GLOB.PIXSIZE), currentPixel);
						pixelsUpdated = true;
						pixelUpdated = true;
					}
					else if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel) && (rand.NextDouble() >= 0.5 || !pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel)))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel);
						pixelsUpdated = true;
						pixelUpdated = true;
					}
					else if(pixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel))
					{
						pixelLib.MovePixel(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel);
						pixelsUpdated = true;
						pixelUpdated = true;
					}
				}

            }

            //Handling for only updating neccesary pixels
            if(pixelUpdated)
            {
				Pixel[] pixNeighbors = pixelLib.GetPixelsInRange(currentPixel.Position, typeof(Pixel), radius: GLOB.PIXELUPDATERADIUS, includeCorners: true, includeSelf: true);
            	foreach(Pixel neighbor in pixNeighbors)
		    	{   
                	GMS.GM.queuePixelUpdate(neighbor);
		    	}
            }
		}

		if(pixelsUpdated || (GMS.GM.GetViewport().GetMousePosition() != lastMousePos))
		{
			HandlePixelTools();
            lastMousePos = GMS.GM.GetViewport().GetMousePosition();
		}
	}

	/// <summary>
	///	The remainder of this code is for handeling our debug interface of spawning pixels on screen, etc.
	///	To be moved to its own file.
	/// </summary>

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
						Pixel newPix = pixelLib.CreatePixel(pixelID, mousePosition);
						if(newPix == null)
						{
							GD.PushError(String.Format("Newly created Pixel was null!"));
							break;
						}
						GMS.GM.AddChild(newPix);
					}
					else if(Input.IsMouseButtonPressed(MouseButton.Right))
					{
                        Vector2 delPos = pixelLib.GetPixelCoordinates(mousePosition);
                        if(GMS.GM.pixelDict.ContainsKey(delPos))
                            {
                                pixelLib.DeletePixel(GMS.GM.pixelDict[delPos]);
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
								Pixel tempPix = pixelLib.CreatePixel(pixelID, new Vector2(mousePosition.X + (x * GLOB.PIXSIZE), mousePosition.Y + (y * GLOB.PIXSIZE)));
								if(tempPix == null)
								{
									//GD.PushError(String.Format("Newly created Pixel was null!"));
									continue;
								}
								GMS.GM.AddChild(tempPix);
							}
							else if(Input.IsMouseButtonPressed(MouseButton.Right))
							{
								Vector2 delPos = pixelLib.GetPixelCoordinates(new Vector2(mousePosition.X + (x * GLOB.PIXSIZE), mousePosition.Y + (y * GLOB.PIXSIZE)));
								if(GMS.GM.pixelDict.ContainsKey(delPos))
									{
										pixelLib.DeletePixel(GMS.GM.pixelDict[delPos]);
									}
							}
						}
					}
					break;
			}
		}
	}
}


