using System;

namespace XamarinApp
{
    public class App
    {
        public static string BackendUrl = "http://localhost:5000";

        public static void Initialize()
        {
            ServiceLocator.Instance.Register<IDataStore<Item>, MockDataStore>();
        }
    }
}
