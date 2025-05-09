using System.Collections.Generic;

namespace WPFFrameworkApp2
{
    internal class WeatherInfo
    {
        public class weather()
        {
            public int id { get; set; }
            public string description { get; set; }
        }

        public class main()
        {
            public double temp { get; set; }
            public double humidity { get; set; }
        }

        public class sys()
        {
            public string country { get; set; }
        }

        public class root()
        {
            public List<weather> weather { get; set; }
            public main main { get; set; }
            public sys sys { get; set; }
            public string name { get; set; }
        }
    }
}
