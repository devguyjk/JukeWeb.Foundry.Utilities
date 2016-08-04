using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JukeWeb.Foundry.Utilities
{
    public class RndGenUtilities
    {
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            return random.Next(min, max);
        }
    }
}
