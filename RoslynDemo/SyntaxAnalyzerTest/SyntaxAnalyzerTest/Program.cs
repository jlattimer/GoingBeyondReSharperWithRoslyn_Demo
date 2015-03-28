using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk;
using System.Configuration;
using Microsoft.Xrm.Sdk.Query;

namespace SyntaxAnalyzerTest
{
    class Program
    {
        private static OrganizationService _orgService;
        static void Main(string[] args)
        {
            CrmConnection connection = CrmConnection.Parse(
                ConfigurationManager.ConnectionStrings["CRMConnectionString"].ConnectionString);

            using (_orgService = new OrganizationService(connection))
            {
                
                Entity account1 = new Entity("account");

                account1["Name"] = "test1";
                
                //C# 6.0
                Entity account2 = new Entity("account") {["Name"] = "test2"};

                Entity account3 = new Entity("account");
                account3.Attributes.Add("Name", "test3"); 
            }
        }
    }
}
