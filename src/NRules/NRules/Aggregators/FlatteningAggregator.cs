using System.Collections.Generic;
using NRules.RuleModel;

namespace NRules.Aggregators
{
    /// <summary>
    /// Aggregator that projects each matching fact into a collection and creates a new fact for each element in that collection.
    /// </summary>
    /// <typeparam name="TSource">Type of source element.</typeparam>
    /// <typeparam name="TResult">Type of result element.</typeparam>
    internal class FlatteningAggregator<TSource, TResult> : IAggregator
    {
        private readonly IAggregateExpression _selector;
        private readonly Dictionary<IFact, IList<TResult>> _sourceToList = new Dictionary<IFact, IList<TResult>>();

        public FlatteningAggregator(IAggregateExpression selector)
        {
            _selector = selector;
        }

        public IEnumerable<AggregationResult> Add(ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var value = (IEnumerable<TResult>)_selector.Invoke(tuple, fact);
                var list = new List<TResult>(value);
                _sourceToList[fact] = list;
                foreach (var item in list)
                {
                    results.Add(AggregationResult.Added(item));
                }
            }
            return results;
        }

        public IEnumerable<AggregationResult> Modify(ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var value = (IEnumerable<TResult>)_selector.Invoke(tuple, fact);
                var list = new List<TResult>(value);
                var oldList = _sourceToList[fact];
                _sourceToList[fact] = list;
                foreach (var item in oldList)
                {
                    results.Add(AggregationResult.Removed(item));
                }
                foreach (var item in list)
                {
                    results.Add(AggregationResult.Added(item));
                }
            }
            return results;
        }

        public IEnumerable<AggregationResult> Remove(ITuple tuple, IEnumerable<IFact> facts)
        {
            var results = new List<AggregationResult>();
            foreach (var fact in facts)
            {
                var oldList = _sourceToList[fact];
                _sourceToList.Remove(fact);
                foreach (var item in oldList)
                {
                    results.Add(AggregationResult.Removed(item));
                }
            }
            return results;
        }
    }
}