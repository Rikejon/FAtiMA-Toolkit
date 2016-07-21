﻿using System.Collections.Generic;
using WellFormedNames;

namespace KnowledgeBase.Conditions
{
	public interface IConditionEvaluator
	{
		IEnumerable<SubstitutionSet> UnifyEvaluate(KB kb, Name perspective, IEnumerable<SubstitutionSet> constraints);

		bool Evaluate(KB kb, Name perspective, IEnumerable<SubstitutionSet> constraints);
	}
}