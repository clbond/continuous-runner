using System;
using System.Collections.Generic;
using System.Linq;
using ContinuousRunner.Data;
using Jint.Parser.Ast;

namespace ContinuousRunner.Extensions
{
    public static class SyntaxExtensions
    {
        public static void Walk<T>(this ExpressionTree<SyntaxNode> expressionTree, Action<T> f)
            where T : SyntaxNode
        {
            Walk(expressionTree.Root, f);
        }

        public static void Walk<T>(this IEnumerable<SyntaxNode> nodes, Action<T> f)
            where T : SyntaxNode
        {
            foreach (var node in nodes)
            {
                Walk(node, f);
            }
        }

        public static void Walk<T>(this SyntaxNode node, Action<T> f) where T : SyntaxNode
        {
            while (true)
            {
                if (node == null)
                {
                    return;
                }

                var matchingType = node as T;
                if (matchingType != null)
                {
                    f?.Invoke(matchingType);
                }

                switch (node.Type)
                {
                    case SyntaxNodes.AssignmentExpression:
                        var assignment = node.As<AssignmentExpression>();
                        Walk(assignment.Left, f);
                        node = assignment.Right;
                        continue;
                    case SyntaxNodes.ArrayExpression:
                        var array = node.As<ArrayExpression>();
                        Walk(array.Elements, f);
                        break;
                    case SyntaxNodes.BinaryExpression:
                        var binaryExpression = node.As<BinaryExpression>();
                        Walk(binaryExpression.Left, f);
                        node = binaryExpression.Right;
                        continue;
                    case SyntaxNodes.CallExpression:
                        var callExpr = node.As<CallExpression>();
                        Walk(callExpr.Callee, f);
                        Walk(callExpr.Arguments, f);
                        break;
                    case SyntaxNodes.CatchClause:
                        var catchClause = node.As<CatchClause>();
                        Walk(catchClause.Param, f);
                        node = catchClause.Body;
                        continue;
                    case SyntaxNodes.ConditionalExpression:
                        var conditional = node.As<ConditionalExpression>();
                        Walk(conditional.Test, f);
                        Walk(conditional.Alternate, f);
                        node = conditional.Consequent;
                        continue;
                    case SyntaxNodes.FunctionDeclaration:
                        var function = node.As<FunctionDeclaration>();
                        Walk(function.Defaults, f);
                        Walk(function.Body, f);
                        Walk(function.Rest, f);
                        Walk(function.Defaults, f);
                        Walk(function.Parameters, f);
                        Walk(function.FunctionDeclarations, f);
                        Walk(function.VariableDeclarations, f);
                        break;
                    case SyntaxNodes.FunctionExpression:
                        var functionExpr = node.As<FunctionExpression>();
                        Walk(functionExpr.Defaults, f);
                        Walk(functionExpr.Body, f);
                        Walk(functionExpr.Rest, f);
                        Walk(functionExpr.Defaults, f);
                        Walk(functionExpr.Parameters, f);
                        Walk(functionExpr.FunctionDeclarations, f);
                        Walk(functionExpr.VariableDeclarations, f);
                        break;
                    case SyntaxNodes.Identifier:
                        break;
                    case SyntaxNodes.Literal:
                    case SyntaxNodes.RegularExpressionLiteral:
                        break;
                    case SyntaxNodes.LogicalExpression:
                        var logicalExpr = node.As<LogicalExpression>();
                        Walk(logicalExpr.Left, f);
                        node = logicalExpr.Right;
                        continue;
                    case SyntaxNodes.MemberExpression:
                        var memberExpr = node.As<MemberExpression>();
                        Walk(memberExpr.Object, f);
                        node = memberExpr.Property;
                        continue;
                    case SyntaxNodes.NewExpression:
                        var newExpr = node.As<NewExpression>();
                        Walk(newExpr.Arguments, f);
                        node = newExpr.Callee;
                        continue;
                    case SyntaxNodes.ObjectExpression:
                        var objExpr = node.As<ObjectExpression>();
                        Walk(objExpr.Properties.Select(p => p.Value), f);
                        break;
                    case SyntaxNodes.Program:
                        var program = node.As<Program>();
                        Walk(program.FunctionDeclarations, f);
                        Walk(program.VariableDeclarations, f);
                        Walk(program.Body, f);
                        break;
                    case SyntaxNodes.Property:
                        var property = node.As<Property>();
                        node = property.Value;
                        continue;
                    case SyntaxNodes.SequenceExpression:
                        var sequence = node.As<SequenceExpression>();
                        Walk(sequence.Expressions, f);
                        break;
                    case SyntaxNodes.SwitchCase:
                        break;
                    case SyntaxNodes.ThisExpression:
                        break;
                    case SyntaxNodes.UnaryExpression:
                        var unary = node.As<UnaryExpression>();
                        node = unary.Argument;
                        continue;
                    case SyntaxNodes.UpdateExpression:
                        var update = node.As<UpdateExpression>();
                        node = update.Argument;
                        continue;
                    case SyntaxNodes.VariableDeclaration:
                        var variable = node.As<VariableDeclaration>();
                        Walk(variable.Declarations, f);
                        break;
                    case SyntaxNodes.VariableDeclarator:
                        var declarator = node.As<VariableDeclarator>();
                        node = declarator.Init;
                        continue;

                    case SyntaxNodes.BlockStatement:
                        var block = node.As<BlockStatement>();
                        Walk(block.Body, f);
                        break;
                    case SyntaxNodes.BreakStatement:
                        break;
                    case SyntaxNodes.ContinueStatement:
                        break;
                    case SyntaxNodes.DoWhileStatement:
                        var doWhile = node.As<DoWhileStatement>();
                        Walk(doWhile.Test, f);
                        node = doWhile.Body;
                        continue;
                    case SyntaxNodes.DebuggerStatement:
                        break;
                    case SyntaxNodes.EmptyStatement:
                        break;
                    case SyntaxNodes.ExpressionStatement:
                        var expr = node.As<ExpressionStatement>();
                        node = expr.Expression;
                        continue;
                    case SyntaxNodes.ForStatement:
                        var forStatement = node.As<ForStatement>();
                        Walk(forStatement.Init, f);
                        Walk(forStatement.Test, f);
                        node = forStatement.Update;
                        continue;
                    case SyntaxNodes.ForInStatement:
                        var forInStatement = node.As<ForInStatement>();
                        Walk(forInStatement.Right, f);
                        Walk(forInStatement.Left, f);
                        node = forInStatement.Body;
                        continue;
                    case SyntaxNodes.IfStatement:
                        var ifStatement = node.As<IfStatement>();
                        Walk(ifStatement.Test, f);
                        Walk(ifStatement.Alternate, f);
                        node = ifStatement.Consequent;
                        continue;
                    case SyntaxNodes.LabeledStatement:
                        var labeledStatement = node.As<LabelledStatement>();
                        node = labeledStatement.Body;
                        continue;
                    case SyntaxNodes.ReturnStatement:
                        var returnStatement = node.As<ReturnStatement>();
                        node = returnStatement.Argument;
                        continue;
                    case SyntaxNodes.SwitchStatement:
                        var switchStatement = node.As<SwitchStatement>();
                        Walk(switchStatement.Discriminant, f);
                        foreach (var switchCase in switchStatement.Cases)
                        {
                            Walk(switchCase.Test, f);
                            Walk(switchCase.Consequent, f);
                        }
                        break;
                    case SyntaxNodes.ThrowStatement:
                        var throwStatement = node.As<ThrowStatement>();
                        node = throwStatement.Argument;
                        continue;
                    case SyntaxNodes.TryStatement:
                        var tryStatement = node.As<TryStatement>();
                        Walk(tryStatement.Block, f);
                        Walk(tryStatement.Finalizer, f);
                        Walk(tryStatement.GuardedHandlers, f);
                        Walk(tryStatement.Handlers, f);
                        break;
                    case SyntaxNodes.WhileStatement:
                        var whileStatement = node.As<WhileStatement>();
                        Walk(whileStatement.Test, f);
                        node = whileStatement.Body;
                        continue;
                    case SyntaxNodes.WithStatement:
                        var withStatement = node.As<WithStatement>();
                        Walk(withStatement.Object, f);
                        node = withStatement.Body;
                        continue;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(node.Type));
                }
                break;
            }
        }

        public static IEnumerable<T> Search<T>(this ExpressionTree<SyntaxNode> tree, Func<T, bool> match)
            where T : SyntaxNode
        {
            return Search(tree.Root, match);
        }

        public static IEnumerable<T> Search<T>(this SyntaxNode node, Func<T, bool> match)
            where T : SyntaxNode
        {
            var results = new List<T>();

            node.Walk<T>(
                s => {
                    if (match(s))
                    {
                        results.Add(s);
                    }
                });

            return results;
        }

        public static T SearchSingle<T>(this SyntaxNode node, Func<T, bool> match)
            where T : SyntaxNode
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

        public static IEnumerable<T> Search<T>(this IEnumerable<SyntaxNode> nodes, Func<T, bool> match)
            where T : SyntaxNode
        {
            var result = new List<T>();

            foreach (var node in nodes)
            {
                result.AddRange(Search<T>(node, match));
            }

            return result;
        }
    }
}