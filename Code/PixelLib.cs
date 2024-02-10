using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public class PixelLib
{	
	//Converts normal vec2 position to pixel grid
	public Vector2 GetPixelCoordinates(Vector2 cords)
	{
		return new Vector2(GLOB.PIXSIZE*((int)(cords.X+GLOB.PIXSIZE-1)/GLOB.PIXSIZE), GLOB.PIXSIZE*((int)(cords.Y+GLOB.PIXSIZE-1)/GLOB.PIXSIZE));
	}
	//Moves a pixel
	public void MovePixel(Vector2 location, pixel targPix, ref Dictionary<Vector2, pixel> pixelDict)
	{	
		if(!PixelCanMoveTo(location, ref pixelDict, targPix))
		{
			GD.PushError(String.Format("{0} failed to be moved to {1}!",targPix, location));
			return;
		}
		location = GetPixelCoordinates(location);
		
		pixelDict.Remove(targPix.Position);
		targPix.Position = location;
		pixelDict.Add(targPix.Position, targPix);

		foreach(pixel neighbor in GetPixelsInRange(targPix.Position, typeof(pixel), ref pixelDict, radius: GLOB.PIXELUPDATERADIUS, includeSelf: true, includeCorners: true))
		{
			neighbor.doUpdate = true;
		}
	}

	//Deletes a pixel
	public void DeletePixel(pixel targPix, ref Dictionary<Vector2, pixel> pixelDict)
	{
		targPix.Position = GetPixelCoordinates(targPix.Position);

		foreach(pixel neighbor in GetPixelsInRange(targPix.Position, typeof(pixel), ref pixelDict, radius: GLOB.PIXELUPDATERADIUS, includeSelf: true, includeCorners: true))
		{
			neighbor.doUpdate = true;
		}

		pixelDict.Remove(targPix.Position);
		targPix.QueueFree();
	}

	//Checks if a pixel can move to a specific position
	public bool PixelCanMoveTo(Vector2 location, ref Dictionary<Vector2, pixel> pixelDict)
	{
		location = GetPixelCoordinates(location);
		
		if(pixelDict.ContainsKey(location))
		{
			return false;
		}
		return true;
	}

	public bool PixelCanMoveTo(Vector2 location, ref Dictionary<Vector2, pixel> pixelDict, pixel targPix)
	{
		location = GetPixelCoordinates(location);
		
		if(pixelDict.ContainsKey(location))
		{
			return false;
		}
		return true;
	}

	public pixel CreatePixel(string pixIDPath, ref Dictionary<Vector2, pixel> pixelDict)
	{
		if(!PixelCanMoveTo(Vector2.Zero, ref pixelDict))
		{
			//GD.PushError(String.Format("Pixel attempted to be created in an invalid position, {0}.",Vector2.Zero));
			return null;
		}
		pixel newPix = (pixel)GD.Load<PackedScene>(pixIDPath).Instantiate();
		pixelDict[newPix.Position] = newPix;

		foreach(pixel neighbor in GetPixelsInRange(newPix.Position, typeof(pixel), ref pixelDict, radius: GLOB.PIXELUPDATERADIUS, includeSelf: true, includeCorners: true))
		{
			neighbor.doUpdate = true;
		}

		return newPix;
	}

	public pixel CreatePixel(string pixIDPath, Vector2 location, ref Dictionary<Vector2, pixel> pixelDict)
	{
		if(!PixelCanMoveTo(location, ref pixelDict))
		{
			//GD.PushError(String.Format("Pixel attempted to be created in an invalid position, {0}.",location));
			return null;
		}
		location = GetPixelCoordinates(location);

		pixel newPix = (pixel)GD.Load<PackedScene>(pixIDPath).Instantiate();
		MovePixel(location, newPix, ref pixelDict);
		return newPix;
	}

	//Returns all pixel neighbors of type T
    public pixel[] GetPixelsInRange(Vector2 location, Type pixType, ref Dictionary<Vector2, pixel> pixelDict, int radius = 1, bool includeCorners = false, bool includeSelf = false)
    {   
        List<pixel> neighbors = new List<pixel>();
        for(int x = -radius; x <= radius; x++)
        {
            for(int y = -radius; y <= radius; y++)
            {
                if(!includeCorners && x != 0 && y != 0)
                {
                    continue;
                }

				if(!includeSelf && x == 0 && y == 0)
				{
					continue;
				}

                Vector2 tempLoc = new Vector2(location.X + (x * GLOB.PIXSIZE),location.Y + (y * GLOB.PIXSIZE));
                if(pixelDict.ContainsKey(tempLoc))
                {
                    if(!GodotObject.IsInstanceValid(pixelDict[tempLoc]))
					{
						pixelDict.Remove(tempLoc);
						continue;
					}
					
					if(!((pixelDict[tempLoc].GetType() == pixType) || pixelDict[tempLoc].GetType().IsSubclassOf(pixType)))
                    {
                        continue;
                    }
                    neighbors.Add(pixelDict[tempLoc]);
                }
            }
        }
        return neighbors.ToArray();
    }
}
