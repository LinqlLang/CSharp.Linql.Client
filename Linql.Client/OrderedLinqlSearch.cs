using Linql.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace Linql.Client
{
    /// <summary>
    /// LinqlSearch represents an IQueryable.  LinqlSearches should mostly be created through an ALinqlContext.
    /// </summary>
    /// <typeparam name="T">The type of the LinqlSearch</typeparam>
    [DebuggerDisplay("OrderedLinqlSearch {ElementType.Name}")]
    public class OrderedLinqlSearch<T> : LinqlSearch<T>, IOrderedQueryable<T>
    {

        public OrderedLinqlSearch() : base()
        {
          
        }

        /// <summary>
        /// Creates a LinqlSearch with the given LinqlContext as its Provider.
        /// </summary>
        /// <param name="Provider">The ALinqlContext Provider</param>
        /// <exception cref="System.Exception">Provider cannot be null</exception>
        public OrderedLinqlSearch(ALinqlContext Provider) : base(Provider)
        {
          
        }


        /// <summary>
        /// Creates a LinqlSearch with a Provider and an Expression
        /// </summary>
        /// <param name="Provider">The ALinqlSearch provider</param>
        /// <param name="Expression">The starting expression of the LinqlSearch</param>
        /// <exception cref="System.Exception">Expression cannot be null</exception>
        public OrderedLinqlSearch(ALinqlContext Provider, Expression Expression) : base(Provider, Expression)
        {
           
        }
    }
}
