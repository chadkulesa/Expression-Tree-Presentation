using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TestWebSite.Models.Home
{
    public class Sample3Child
    {
        public Sample3Child(int count)
        {
            _count = count;
        }

        private int _count;

        public int GetCount()
        {
            return _count;
        }
    }
}