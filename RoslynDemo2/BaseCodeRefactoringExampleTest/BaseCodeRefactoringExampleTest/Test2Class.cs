using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseCodeRefactoringExampleTest
{
    class Test2Class
    {
        private void Test()
        {
            Test1Class test1 = new Test1Class();

            string x = test1.MyProperty;
        }      
    }
}
