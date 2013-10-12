using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace NRules.Rule.Builders
{
    public class PatternBuilder : RuleElementBuilder, IRuleElementBuilder<PatternElement>
    {
        private readonly Declaration _declaration;
        private readonly List<ConditionElement> _conditions = new List<ConditionElement>();

        internal PatternBuilder(Declaration declaration, SymbolTable parentScope) : base(parentScope)
        {
            _declaration = declaration;
        }

        public void Condition(LambdaExpression expression)
        {
            var declarations = expression.Parameters.Select(p => Scope.Lookup(p.Name, p.Type));
            var condition = new ConditionElement(declarations, expression);
            _conditions.Add(condition);
        }

        PatternElement IRuleElementBuilder<PatternElement>.Build()
        {
            var patternElement = new PatternElement(_declaration.Type);
            _declaration.Target = patternElement;
            _conditions.ForEach(patternElement.Add);
            return patternElement;
        }
    }
}