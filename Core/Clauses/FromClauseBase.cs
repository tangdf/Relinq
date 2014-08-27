// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Diagnostics;
using System.Linq.Expressions;
using Remotion.Linq.Clauses.Expressions;
using Remotion.Linq.Clauses.ExpressionTreeVisitors;
using Remotion.Utilities;

namespace Remotion.Linq.Clauses
{
  /// <summary>
  /// Base class for from clauses (<see cref="AdditionalFromClause"/> and <see cref="MainFromClause"/>). From clauses define query sources that
  /// provide data items to the query which are filtered, ordered, projected, or otherwise processed by the following clauses.
  /// </summary>
  public abstract class FromClauseBase : IClause, IQuerySource
  {
    private string _itemName;
    private Type _itemType;
    private Expression _fromExpression;

    /// <summary>
    /// Initializes a new instance of the <see cref="FromClauseBase"/> class.
    /// </summary>
    /// <param name="itemName">A name describing the items generated by the from clause.</param>
    /// <param name="itemType">The type of the items generated by the from clause.</param>
    /// <param name="fromExpression">The <see cref="Expression"/> generating data items for this from clause.</param>
    protected FromClauseBase (string itemName, Type itemType, Expression fromExpression)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("itemName", itemName);
      ArgumentUtility.CheckNotNull ("itemType", itemType);
      ArgumentUtility.CheckNotNull ("fromExpression", fromExpression);

      _itemName = itemName;
      _itemType = itemType;
      _fromExpression = fromExpression;
    }

    /// <summary>
    /// Gets or sets a name describing the items generated by this from clause.
    /// </summary>
    /// <remarks>
    /// Item names are inferred when a query expression is parsed, and they usually correspond to the variable names present in that expression. 
    /// However, note that names are not necessarily unique within a <see cref="QueryModel"/>. Use names only for readability and debugging, not for 
    /// uniquely identifying <see cref="IQuerySource"/> objects. To match an <see cref="IQuerySource"/> with its references, use the 
    /// <see cref="QuerySourceReferenceExpression.ReferencedQuerySource"/> property rather than the <see cref="ItemName"/>.
    /// </remarks>
    public string ItemName
    {
      get { return _itemName; }
      set { _itemName = ArgumentUtility.CheckNotNullOrEmpty ("value", value); }
    }

    /// <summary>
    /// Gets or sets the type of the items generated by this from clause.
    /// </summary>
    /// <note type="warning">
    /// Changing the <see cref="ItemType"/> of a <see cref="IQuerySource"/> can make all <see cref="QuerySourceReferenceExpression"/> objects that
    /// point to that <see cref="IQuerySource"/> invalid, so the property setter should be used with care.
    /// </note>
    public Type ItemType
    {
      get { return _itemType; }
      set { _itemType = ArgumentUtility.CheckNotNull ("value", value); }
    }

    /// <summary>
    /// The expression generating the data items for this from clause.
    /// </summary>
    [DebuggerDisplay ("{Remotion.Linq.Clauses.ExpressionTreeVisitors.FormattingExpressionTreeVisitor.Format (FromExpression),nq}")]
    public Expression FromExpression
    {
      get { return _fromExpression; }
      set { _fromExpression = ArgumentUtility.CheckNotNull ("value", value); }
    }

    /// <summary>
    /// Transforms all the expressions in this clause and its child objects via the given <paramref name="transformation"/> delegate.
    /// </summary>
    /// <param name="transformation">The transformation object. This delegate is called for each <see cref="Expression"/> within this
    /// clause, and those expressions will be replaced with what the delegate returns.</param>
    public virtual void TransformExpressions (Func<Expression, Expression> transformation)
    {
      ArgumentUtility.CheckNotNull ("transformation", transformation);
      FromExpression = transformation (FromExpression);
    }

    public override string ToString ()
    {
      var result = string.Format ("from {0} {1} in {2}", ItemType.Name, ItemName, FormattingExpressionTreeVisitor.Format (FromExpression));
      return result;
    }

  }
}