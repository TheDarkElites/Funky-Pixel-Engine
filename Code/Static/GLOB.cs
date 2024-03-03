//Serves as a class to allow us to have global vars/defines

public static class GLOB
{
    //Size of our pixels
    public static int PIXSIZE = 4;
    //How many seconds must pass for pixels to update
    public static double PHYSSPEED = 0.025;
    //How large the radius of an update is when pixels update;
    public static int PIXELUPDATERADIUS = 2;
    //Temperature transfer multiplier
    public static double TEMPTRANSMULT = 1.0;
    //Temperature conductivity for air (no pixel)
    public static double AIRTHEMCONDUCT = 0.6;
    
    //World Bounds. Keep in mind Y+ is down.
    public static double WRLDMINX = 0;
    public static double WRLDMAXX = 1000;
    public static double WRLDMINY = 0;
    public static double WRLDMAXY = 1000;
}
