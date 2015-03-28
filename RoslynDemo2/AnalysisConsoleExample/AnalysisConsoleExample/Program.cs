using System;
using Microsoft.CodeAnalysis;

namespace AnalysisConsoleExample
{
	class Program
	{
		static void Main(string[] args)
		{
			AdhocWorkspace workspace = new AdhocWorkspace();
			Solution solution = workspace.CurrentSolution;
			Project project = solution.AddProject("projectName", "assemblyName", LanguageNames.CSharp);
			Document document = project.AddDocument("name.cs",
																@"class Class1
																  {
																	  static void Main()
																	  {
																		  WriteLine(""Hello, World!"");
																	  }
																  }");

			var root = document.GetSyntaxRootAsync().Result;

			foreach (SyntaxNode node in root.ChildNodes())
			{
				if (node != null)
					Console.WriteLine(node.ToString());
			}

		    Console.ReadLine();
		}
	}
}
