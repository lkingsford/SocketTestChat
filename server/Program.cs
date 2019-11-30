namespace server
{
    class Program
    {
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            Logger.Info("Server Started");

            var server = new Server();
            server.Start();
        }

}
}
