using Godot;
using System;
using System.Collections.Generic;

public partial class BasePixelPhysicsSystem : Subsystem
{	
    public BasePixelPhysicsSystem()
    {

    }

	// Called every frame. 'delta' is the elapsed time since the previous frame. Returns a list of pixels to update
	public override void Fire(Stack<Pixel> UpdateStack)
	{
		//Begin Pixel simulation
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
			if(currentPixel.Position.Y > GLOB.WRLDMAXY || currentPixel.Position.Y <= GLOB.WRLDMINY)
			{
                PixelLib.DeletePixel(currentPixel);
                pixelUpdated = true;
			}

            if(currentPixel.isStatic) //Dont need to update static pixels or pixels not scheduled to update
            {
				continue;
            }
			
            //Gravity Calculations
            //Amount of required connections to pixels to avoid us from falling
			int reqConnections = (int)Math.Ceiling(8 - Math.Clamp(currentPixel.ductility * 16, 0, 4));
            if(PixelLib.GetPixelsInRange(currentPixel.Position, currentPixel.GetType(), includeCorners: true).Length < reqConnections)
            {
                if(currentPixel is Solid || currentPixel is Liquid)
				{
					if(PixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X, currentPixel.Position.Y + GLOB.PIXSIZE), currentPixel))
					{
						PixelLib.MovePixel(new Vector2(currentPixel.Position.X, currentPixel.Position.Y + GLOB.PIXSIZE), currentPixel);
						pixelUpdated = true;
					} 
					else if(PixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y + GLOB.PIXSIZE), currentPixel) && (rand.Next(1,101) <= (100 * (1 - currentPixel.ductility))))
					{
						PixelLib.MovePixel(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y + GLOB.PIXSIZE), currentPixel);
						pixelUpdated = true;
					}
					else if(PixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y + GLOB.PIXSIZE), currentPixel) && (rand.Next(1,101) <= (100 * (1 - currentPixel.ductility))))
					{
						PixelLib.MovePixel(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y + GLOB.PIXSIZE), currentPixel);
						pixelUpdated = true;
					}
				}

				if(currentPixel is Liquid && !pixelUpdated)
				{
					if(PixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel) && (rand.NextDouble() >= 0.5 || !PixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel)))
					{
						PixelLib.MovePixel(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel);
						pixelUpdated = true;
					}
					else if(PixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel))
					{
						PixelLib.MovePixel(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel);
						pixelUpdated = true;
					}
				}

				if(currentPixel is Gas)
				{
					if(PixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X, currentPixel.Position.Y - GLOB.PIXSIZE), currentPixel))
					{
						PixelLib.MovePixel(new Vector2(currentPixel.Position.X, currentPixel.Position.Y - GLOB.PIXSIZE), currentPixel);
						pixelUpdated = true;
					} 
					else if(PixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y - GLOB.PIXSIZE), currentPixel) && (rand.Next(1,101) <= (100 * (1 - currentPixel.ductility))))
					{
						PixelLib.MovePixel(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y - GLOB.PIXSIZE), currentPixel);
						pixelUpdated = true;
					}
					else if(PixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y - GLOB.PIXSIZE), currentPixel) && (rand.Next(1,101) <= (100 * (1 - currentPixel.ductility))))
					{
						PixelLib.MovePixel(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y - GLOB.PIXSIZE), currentPixel);
						pixelUpdated = true;
					}
					else if(PixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel) && (rand.NextDouble() >= 0.5 || !PixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel)))
					{
						PixelLib.MovePixel(new Vector2(currentPixel.Position.X + GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel);
						pixelUpdated = true;
					}
					else if(PixelLib.PixelCanMoveTo(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel))
					{
						PixelLib.MovePixel(new Vector2(currentPixel.Position.X - GLOB.PIXSIZE, currentPixel.Position.Y), currentPixel);
						pixelUpdated = true;
					}
				}

            }

            //Handling for only updating neccesary pixels
            if(pixelUpdated)
            {
				Pixel[] pixNeighbors = PixelLib.GetPixelsInRange(currentPixel.Position, typeof(Pixel), radius: GLOB.PIXELUPDATERADIUS, includeCorners: true, includeSelf: true);
            	foreach(Pixel neighbor in pixNeighbors)
		    	{   
                	GMS.GM.queuePixelUpdate(neighbor);
		    	}
            }
		}
	}
}


