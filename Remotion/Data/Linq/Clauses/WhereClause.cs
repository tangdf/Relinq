// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Remotion.Data.Linq.Clauses.Expressions;
using Remotion.Data.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Data.Linq.StringBuilding;
using Remotion.Utilities;

namespace Remotion.Data.Linq.Clauses
{
  /// <summary>
  /// Represents the where part of a query, filtering data items according to some <see cref="Predicate"/>.
  /// </summary>
  /// <example>
  /// In C#, the "where" clause in the following sample corresponds to a <see cref="WhereClause"/>:
  /// <ode>
  /// var query = from s in Students
  ///             where s.First == "Hugo"
  ///             select s;
  /// </ode>
  /// </example>
  public class WhereClause : IBodyClause
  {
    private Expression _predicate;

    /// <summary>
    /// Initializes a new instance of the <see cref="WhereClause"/> class.
    /// </summary>
    /// <param name="predicate">The predicate used to filter data items.</param>
    public WhereClause (Expression predicate)
    {
      ArgumentUtility.CheckNotNull ("predicate", predicate);
      _predicate = predicate;
    }

    /// <summary>
    /// Gets the predicate, the expression representing the where condition by which the data items are filtered
    /// </summary>
    [DebuggerDisplay ("{Remotion.Data.Linq.StringBuilding.FormattingExpressionTreeVisitor.Format (Predicate),nq}")]
    public Expression Predicate
    {
      get { return _predicate; }
      set { _predicate = ArgumentUtility.CheckNotNull ("value", value); }
    }

    /// <summary>
    /// Accepts the specified visitor by calling its <see cref="IQueryModelVisitor.VisitWhereClause"/> method.
    /// </summary>
    /// <param name="visitor">The visitor to accept.</param>
    /// <param name="queryModel">The query model in whose context this clause is visited.</param>
    /// <param name="index">The index of this clause in the <paramref name="queryModel"/>'s <see cref="QueryModel.BodyClauses"/> collection.</param>
    public virtual void Accept (IQueryModelVisitor visitor, QueryModel queryModel, int index)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      ArgumentUtility.CheckNotNull ("queryModel", queryModel);

      visitor.VisitWhereClause (this, queryModel, index);
    }

    /// <summary>
    /// Transforms all the expressions in this clause and its child objects via the given <paramref name="transformation"/> delegate.
    /// </summary>
    /// <param name="transformation">The transformation object. This delegate is called for each <see cref="Expression"/> within this
    /// clause, and those expressions will be replaced with what the delegate returns.</param>
    public void TransformExpressions (Func<Expression, Expression> transformation)
    {
      ArgumentUtility.CheckNotNull ("transformation", transformation);
      Predicate = transformation (Predicate);
    }

    /// <summary>
    /// Clones this clause, adjusting all <see cref="QuerySourceReferenceExpression"/> instances held by it as defined by
    /// <paramref name="cloneContext"/>.
    /// </summary>
    /// <param name="cloneContext">The clone context to use for replacing <see cref="QuerySourceReferenceExpression"/> objects.</param>
    /// <returns></returns>
    public WhereClause Clone (CloneContext cloneContext)
    {
      ArgumentUtility.CheckNotNull ("cloneContext", cloneContext);

      var clone = new WhereClause (Predicate);
      clone.TransformExpressions (ex => ReferenceReplacingExpressionTreeVisitor.ReplaceClauseReferences (ex, cloneContext.ClauseMapping));
      return clone;
    }

    IBodyClause IBodyClause.Clone (CloneContext cloneContext)
    {
      return Clone (cloneContext);
    }

    public override string ToString ()
    {
      return "where " + FormattingExpressionTreeVisitor.Format (Predicate);
    }
  }
}
