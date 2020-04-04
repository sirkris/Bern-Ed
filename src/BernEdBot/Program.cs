namespace BernEdBot
{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO - For prod, enclose in try/catch loop so bot never dies.  --Kris
            (new Workflow()).MainLoop();
        }
    }
}
