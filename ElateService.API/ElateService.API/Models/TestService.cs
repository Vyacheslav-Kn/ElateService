using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace NinjectApi.Models
{
    public class TestService: ITestService
    {
        public string GetSomething()
        {
            return "Fuck";
        }
    }
}