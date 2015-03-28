using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;

namespace MultiRefactoringTest
{
	class Program
	{
		static void Main(string[] args)
		{
			var i1 = 0;

			Tester(i1);
		}

		private static void Tester(int i)
		{
			string t1 = "Hello World";

			var i2 = 6;

            Entity e = new Entity();

			//Do Stuff...
		}
	}
}
