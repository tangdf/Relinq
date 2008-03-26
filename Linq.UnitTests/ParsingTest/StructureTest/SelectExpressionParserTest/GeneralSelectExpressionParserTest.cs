using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using Rubicon.Data.Linq.Parsing;
using Rubicon.Data.Linq.Parsing.Structure;
using Rubicon.Data.Linq.UnitTests.TestQueryGenerators;

namespace Rubicon.Data.Linq.UnitTests.ParsingTest.StructureTest.SelectExpressionParserTest
{
  [TestFixture]
  public class GeneralSelectExpressionParserTest
  {

    [Test]
    public void Initialize ()
    {
      IQueryable<Student> querySource = ExpressionHelper.CreateQuerySource();
      MethodCallExpression expression = SelectTestQueryGenerator.CreateSimpleQuery_SelectExpression (querySource);
      new SelectExpressionParser ().Parse (new ParseResultCollector(expression), expression);
    }

    [Test]
    [ExpectedException (typeof (ParserException), ExpectedMessage = "Expected one of 'Select', but found 'Where' at position "
        + "value(Rubicon.Data.Linq.UnitTests.TestQueryable`1[Rubicon.Data.Linq.UnitTests."
            + "Student]).Where(s => (s.Last = \"Garcia\")) in tree value(Rubicon.Data.Linq.UnitTests.TestQueryable`1"
                + "[Rubicon.Data.Linq.UnitTests.Student]).Where(s => (s.Last = \"Garcia\")).")]
    public void Initialize_FromWrongExpression ()
    {
      MethodCallExpression expression = WhereTestQueryGenerator.CreateSimpleWhereQuery_WhereExpression (ExpressionHelper.CreateQuerySource ());
      new SelectExpressionParser ().Parse (new ParseResultCollector (expression), expression);
    }

    [Test]
    [ExpectedException (typeof (ParserException), ExpectedMessage = "Expected Constant or Call expression for first argument of Select "
        + "expression, found UnaryExpression (Convert(null)).")]
    public void Initialize_FromWrongExpressionInWhereExpression ()
    {
      Expression nonCallExpression = Expression.Convert (Expression.Constant (null), typeof (IQueryable<Student>));
      // Get method Queryable.Select whose second argument is of type Expression<Func<TSource, TResult>>
      // (rather than Expression<Func<TSource, int, TResult>>)
      MethodInfo method = (from m in typeof (Queryable).GetMethods () where m.Name == "Select"
          && m.GetParameters()[1].ParameterType.GetGenericArguments()[0].GetGenericArguments().Length == 2 select m).First ();
      method = method.MakeGenericMethod (typeof (Student), typeof (Student));
      MethodCallExpression selectExpression = Expression.Call (method, nonCallExpression, Expression.Lambda (Expression.Constant (null, typeof (Student)), Expression.Parameter (typeof (Student), "student")));
      new SelectExpressionParser ().Parse (new ParseResultCollector (selectExpression), selectExpression);
    }

    
  }
}