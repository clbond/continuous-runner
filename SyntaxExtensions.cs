using System;
using System.Collections.Generic;
using System.Linq;
using Jint.Parser.Ast;

namespace TestRunner
{
    public static class SyntaxExtensions
    {
        public static void Walk<T>(this IEnumerable<SyntaxNode> nodes, Action<T> f) where T : SyntaxNode
        {
            foreach (var node in nodes)
            {
                Walk(node, f);
            }
        }

        public static void Walk<T>(this SyntaxNode node, Action<T> f) where T : SyntaxNode
        {
            if (node == null)
            {
                return;
            }

            var matchingType = node as T;
            if (matchingType != null)
            {
                f(matchingType);
            }

            switch (node.Type)
            {
                case SyntaxNodes.AssignmentExpression:
                    var assignment = node.As<AssignmentExpression>();
                    Walk<T>(assignment.Left, f);
                    Walk<T>(assignment.Right, f);
                    break;
                case SyntaxNodes.ArrayExpression:
                    var array = node.As<ArrayExpression>();
                    Walk<T>(array.Elements, f);
                    break;
                case SyntaxNodes.BinaryExpression:
                    var binaryExpression = node.As<BinaryExpression>();
                    Walk<T>(binaryExpression.Left, f);
                    Walk<T>(binaryExpression.Right, f);
                    break;
                case SyntaxNodes.CallExpression:
                    var callExpr = node.As<CallExpression>();
                    Walk<T>(callExpr.Callee, f);
                    Walk<T>(callExpr.Arguments, f);
                    break;
                case SyntaxNodes.CatchClause:
                    var catchClause = node.As<CatchClause>();
                    Walk<T>(catchClause.Param, f);
                    Walk<T>(catchClause.Body, f);
                    break;
                case SyntaxNodes.ConditionalExpression:
                    var conditional = node.As<ConditionalExpression>();
                    Walk<T>(conditional.Test, f);
                    Walk<T>(conditional.Alternate, f);
                    Walk<T>(conditional.Consequent, f);
                    break;
                case SyntaxNodes.FunctionDeclaration:
                    var function = node.As<FunctionDeclaration>();
                    Walk<T>(function.Defaults, f);
                    Walk<T>(function.Body, f);
                    Walk<T>(function.Rest, f);
                    Walk<T>(function.Defaults, f);
                    Walk<T>(function.Parameters, f);
                    Walk<T>(function.FunctionDeclarations, f);
                    Walk<T>(function.VariableDeclarations, f);
                    break;
                case SyntaxNodes.FunctionExpression:
                    var functionExpr = node.As<FunctionExpression>();
                    Walk<T>(functionExpr.Defaults, f);
                    Walk<T>(functionExpr.Body, f);
                    Walk<T>(functionExpr.Rest, f);
                    Walk<T>(functionExpr.Defaults, f);
                    Walk<T>(functionExpr.Parameters, f);
                    Walk<T>(functionExpr.FunctionDeclarations, f);
                    Walk<T>(functionExpr.VariableDeclarations, f);
                    break;
                case SyntaxNodes.Identifier:
                    break;
                case SyntaxNodes.Literal:
                case SyntaxNodes.RegularExpressionLiteral:
                    break;
                case SyntaxNodes.LogicalExpression:
                    var logicalExpr = node.As<LogicalExpression>();
                    Walk<T>(logicalExpr.Left, f);
                    Walk<T>(logicalExpr.Right, f);
                    break;
                case SyntaxNodes.MemberExpression:
                    var memberExpr = node.As<MemberExpression>();
                    Walk<T>(memberExpr.Object, f);
                    Walk<T>(memberExpr.Property, f);
                    break;
                case SyntaxNodes.NewExpression:
                    var newExpr = node.As<NewExpression>();
                    Walk<T>(newExpr.Arguments, f);
                    Walk<T>(newExpr.Callee, f);
                    break;
                case SyntaxNodes.ObjectExpression:
                    var objExpr = node.As<ObjectExpression>();
                    Walk<T>(objExpr.Properties.Select(p => p.Value), f);
                    break;
                case SyntaxNodes.Program:
                    var program = node.As<Program>();
                    Walk<T>(program.FunctionDeclarations, f);
                    Walk<T>(program.VariableDeclarations, f);
                    Walk<T>(program.Body, f);
                    break;
                case SyntaxNodes.Property:
                    var property = node.As<Property>();
                    Walk<T>(property.Value, f);
                    break;
                case SyntaxNodes.SequenceExpression:
                    var sequence = node.As<SequenceExpression>();
                    Walk<T>(sequence.Expressions, f);
                    break;
                case SyntaxNodes.SwitchCase:
                    break;
                case SyntaxNodes.ThisExpression:
                    break;
                case SyntaxNodes.UnaryExpression:
                    var unary = node.As<UnaryExpression>();
                    Walk<T>(unary.Argument, f);
                    break;
                case SyntaxNodes.UpdateExpression:
                    var update = node.As<UpdateExpression>();
                    Walk<T>(update.Argument, f);
                    break;
                case SyntaxNodes.VariableDeclaration:
                    var variable = node.As<VariableDeclaration>();
                    Walk<T>(variable.Declarations, f);
                    break;
                case SyntaxNodes.VariableDeclarator:
                    var declarator = node.As<VariableDeclarator>();
                    Walk<T>(declarator.Init, f);
                    break;

                case SyntaxNodes.BlockStatement:
                    var block = node.As<BlockStatement>();
                    Walk<T>(block.Body, f);
                    break;
                case SyntaxNodes.BreakStatement:
                    break;
                case SyntaxNodes.ContinueStatement:
                    break;
                case SyntaxNodes.DoWhileStatement:
                    var doWhile = node.As<DoWhileStatement>();
                    Walk<T>(doWhile.Test, f);
                    Walk<T>(doWhile.Body, f);
                    break;
                case SyntaxNodes.DebuggerStatement:
                    break;
                case SyntaxNodes.EmptyStatement:
                    break;
                case SyntaxNodes.ExpressionStatement:
                    var expr = node.As<ExpressionStatement>();
                    Walk<T>(expr.Expression, f);
                    break;
                case SyntaxNodes.ForStatement:
                    var forStatement = node.As<ForStatement>();
                    Walk<T>(forStatement.Init, f);
                    Walk<T>(forStatement.Test, f);
                    Walk<T>(forStatement.Update, f);
                    break;
                case SyntaxNodes.ForInStatement:
                    var forInStatement = node.As<ForInStatement>();
                    Walk<T>(forInStatement.Right, f);
                    Walk<T>(forInStatement.Left, f);
                    Walk<T>(forInStatement.Body, f);
                    break;
                case SyntaxNodes.IfStatement:
                    var ifStatement = node.As<IfStatement>();
                    Walk<T>(ifStatement.Test, f);
                    Walk<T>(ifStatement.Alternate, f);
                    Walk<T>(ifStatement.Consequent, f);
                    break;
                case SyntaxNodes.LabeledStatement:
                    var labeledStatement = node.As<LabelledStatement>();
                    Walk<T>(labeledStatement.Body, f);
                    break;
                case SyntaxNodes.ReturnStatement:
                    var returnStatement = node.As<ReturnStatement>();
                    Walk<T>(returnStatement.Argument, f);
                    break;
                case SyntaxNodes.SwitchStatement:
                    var switchStatement = node.As<SwitchStatement>();
                    Walk<T>(switchStatement.Discriminant, f);
                    foreach (var switchCase in switchStatement.Cases)
                    {
                        Walk<T>(switchCase.Test, f);
                        Walk<T>(switchCase.Consequent, f);
                    }
                    break;
                case SyntaxNodes.ThrowStatement:
                    var throwStatement = node.As<ThrowStatement>();
                    Walk<T>(throwStatement.Argument, f);
                    break;
                case SyntaxNodes.TryStatement:
                    var tryStatement = node.As<TryStatement>();
                    Walk<T>(tryStatement.Block, f);
                    Walk<T>(tryStatement.Finalizer, f);
                    Walk<T>(tryStatement.GuardedHandlers, f);
                    Walk<T>(tryStatement.Handlers, f);
                    break;
                case SyntaxNodes.WhileStatement:
                    var whileStatement = node.As<WhileStatement>();
                    Walk<T>(whileStatement.Test, f);
                    Walk<T>(whileStatement.Body, f);
                    break;
                case SyntaxNodes.WithStatement:
                    var withStatement = node.As<WithStatement>();
                    Walk<T>(withStatement.Object, f);
                    Walk<T>(withStatement.Body, f);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(node.Type));
            }
        }

        public static IEnumerable<T> Search<T>(this SyntaxNode node, Func<T, bool> match) where T : SyntaxNode
        {
            var results = new List<T>();

            node.Walk<T>(s =>
                         {
                             if (match(s))
                             {
                                 results.Add(s);
                             }
                         });

            return results;
        }

        public static T SearchSingle<T>(this SyntaxNode node, Func<T, bool> match) where T : SyntaxNode
        {
            var results = new List<T>();

            node.Walk<T>(s =>
                         {
                             if (match(s))
                             {
                                 results.Add(s);
                             }
                         });

            return results.FirstOrDefault();
        }
    }
}