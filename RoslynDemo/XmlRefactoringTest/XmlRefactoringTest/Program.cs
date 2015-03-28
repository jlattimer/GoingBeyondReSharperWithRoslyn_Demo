using Microsoft.Xrm.Client;
using Microsoft.Xrm.Client.Services;
using Microsoft.Xrm.Sdk.Query;
using System.Configuration;

namespace XmlRefactoringTest
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
                FetchExpression query1 = new FetchExpression(@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
				                                                <entity name='account'>
				                                                  <attribute name='name' />
				                                                  <attribute name='primarycontactid' />
				                                                  <attribute name='telephone1' />
				                                                  <attribute name='accountid' />
				                                                  <order attribute='name' descending='false' /></entity></fetch>");

                string query2 = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                <entity name='account'>
                                  <attribute name='name' />
                                  <attribute name='primarycontactid' />
                                  <attribute name='telephone1' />
                                  <attribute name='accountid' />
                                  <order attribute='name' descending='false' /></entity></fetch>";
            }
        }
    }
}
