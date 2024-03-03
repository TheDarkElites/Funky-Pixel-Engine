//Serves as a method of accessing our game managment systems really easily

public static class GMS
{
    public static readonly GameManager GM = new GameManager();
    public static readonly BasePixelPhysicsSystem BPPS = new BasePixelPhysicsSystem();
    public static readonly AmbientForceSystem AFS = new AmbientForceSystem();
    public static readonly DebugSystem DBS = new DebugSystem();
}