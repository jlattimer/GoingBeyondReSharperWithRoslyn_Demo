using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CodeRefactoring1
{
	[ExportCodeRefactoringProvider(LanguageNames.CSharp, Name = nameof(MultiRefactoringCodeRefactoringProvider)), Shared]
	internal class MultiRefactoringCodeRefactoringProvider : CodeRefactoringProvider
	{
		public sealed override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
			var node = root.FindNode(context.Span);

			// Only offer a refactoring if the selected node is a type VariableDeclarator node.
			if (!node.IsKind(SyntaxKind.VariableDeclarator)) return;
			var typeDecl = (VariableDeclaratorSyntax)node;

			var action = CodeAction.Create("Make parameter", c => MakeParameterAsync(context.Document, typeDecl, c));

			context.RegisterRefactoring(action);
		}

		private async Task<Document> MakeParameterAsync(Document document, VariableDeclaratorSyntax typeDecl, CancellationToken cancellationToken)
		{
			var root = await document.GetSyntaxRootAsync(cancellationToken);
			string varKeyword = string.Empty, varName = string.Empty;

			//Predefined variable type (string, int, etc...)
			var preDefType = typeDecl.Parent.ChildNodes().OfType<PredefinedTypeSyntax>().FirstOrDefault();
			if (preDefType != null)
			{
				varKeyword = preDefType.Keyword.ToString();
				var varDecl = typeDecl.Parent.ChildNodes().OfType<VariableDeclaratorSyntax>().FirstOrDefault();
				varName = varDecl?.Identifier.ToString();
			}
			else //var
			{
				var identName = typeDecl.Parent.ChildNodes().OfType<IdentifierNameSyntax>().FirstOrDefault();

				//Access the semantic model to determine actual type
				var model = document.GetSemanticModelAsync().Result;
                var type = model.GetTypeInfo(identName).Type;
				varKeyword = type.ToMinimalDisplayString(model, typeDecl.SpanStart);

				var varDecl = typeDecl.Parent.ChildNodes().OfType<VariableDeclaratorSyntax>().FirstOrDefault();
				varName = varDecl?.Identifier.ToString();
			}

			MethodDeclarationSyntax mds = typeDecl.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();

			//Add the existing and new parameters
			ParameterSyntax ps = SyntaxFactory.Parameter(SyntaxFactory.Identifier(string.Concat(varKeyword, " ", varName)));
			var newList = SyntaxFactory.ParameterList(SyntaxFactory.SeparatedList<ParameterSyntax>().AddRange(
				mds.ParameterList.ChildNodes().OfType<ParameterSyntax>()).Add(ps));

			var methodNode = typeDecl.Ancestors().OfType<MethodDeclarationSyntax>().FirstOrDefault();
			var variableNode = typeDecl.Parent.Parent;
			var variableParentNode = typeDecl.Parent.Parent.Parent;

			//Track the nodes we'll be using 
			var newRoot = root.TrackNodes(methodNode, variableNode, variableParentNode);

			//Remode/replace the variable declaration
			var trackedVariableParentNode = newRoot.GetCurrentNode(variableParentNode);
			var trackedVariableNode = newRoot.GetCurrentNode(variableNode);
			var newVariableParentNode = trackedVariableParentNode.RemoveNode(trackedVariableNode, SyntaxRemoveOptions.KeepNoTrivia);
			newRoot = newRoot.ReplaceNode(trackedVariableParentNode, newVariableParentNode);

			//Replace the method parameters
			var trackedMethodNode = newRoot.GetCurrentNode(methodNode);
			var newMethodNode = trackedMethodNode.ReplaceNode(trackedMethodNode.ParameterList, newList);
			newRoot = newRoot.ReplaceNode(trackedMethodNode, newMethodNode);

			return document.WithSyntaxRoot(newRoot);
		}
	}
}