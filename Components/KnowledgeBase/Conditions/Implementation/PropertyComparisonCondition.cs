﻿using System;
using System.Collections.Generic;
using KnowledgeBase.WellFormedNames;

namespace KnowledgeBase.Conditions
{
	public partial class Condition
	{
		private sealed class PropertyComparisonCondition : Condition
		{
			private readonly ComparisonOperator Operator;
			private readonly Name Property1;
			private readonly Name Property2;

			public PropertyComparisonCondition(Name property1, Name property2, ComparisonOperator op)
			{
				Property1 = property1;
				Property2 = property2;
				Operator = op;
			}

			public override IEnumerable<SubstitutionSet> UnifyEvaluate(KB kb, SubstitutionSet constraints)
			{
				if (constraints == null)
					constraints = new SubstitutionSet();

				Name p1 = Property1;
				if (!p1.IsGrounded)
					p1 = p1.MakeGround(constraints);

				Name p2 = Property2;
				if (!p2.IsGrounded)
					p2 = p2.MakeGround(constraints);

				if (p1.IsGrounded == p2.IsGrounded)
				{
					if (p1.IsGrounded)
					{
						var v1 = kb.AskProperty(p1);
						if (v1 != null)
						{
							var v2 = kb.AskProperty(p2);
							if (v2 != null)
							{
								if (CompareValues(v1, v2, Operator))
									yield return constraints;
							}
						}
					}
					else
					{
						foreach (var pair in kb.AskPossibleProperties(p1, constraints))
						{
							foreach (var crossPair in kb.AskPossibleProperties(p2, pair.Item2))
							{
								if (CompareValues(pair.Item1, crossPair.Item1, Operator))
									yield return crossPair.Item2;
							}
						}
					}
				}
				else
				{
					Name ungrounded = p1.IsGrounded ? p2 : p1;
					Name grounded = p1.IsGrounded ? p1 : p2;
					ComparisonOperator op = p1.IsGrounded ? Operator : Operator.Mirror();

					var value = kb.AskProperty(grounded) ?? grounded.ToString();
					foreach (var pair in kb.AskPossibleProperties(ungrounded, constraints))
					{
						if (CompareValues(value, pair.Item1, op))
							yield return pair.Item2;
					}
				}
			}

			public override bool Equals(object obj)
			{
				PropertyComparisonCondition c = obj as PropertyComparisonCondition;
				if (c == null)
					return false;

				Name p1 = c.Property1;
				Name p2 = c.Property2;
				var op = c.Operator;

				if (Operator != op)
				{
					op = op.Mirror();
					if (Operator != op)
						return false;
					var s = p1;
					p1 = p2;
					p2 = s;
				}

				switch (op)
				{
					case ComparisonOperator.Equal:
					case ComparisonOperator.NotEqual:
						return (Property1 == p1 && Property2 == p2) || (Property1 == p2 && Property2 == p1);
				}
				return Property1 == p1 && Property2 == p2;
			}

			public override int GetHashCode()
			{
				Name p1 = Property1;
				Name p2 = Property2;
				var op = Operator;
				switch (op)
				{
					case ComparisonOperator.GreatherThan:
					case ComparisonOperator.GreatherOrEqualThan:
						op = op.Mirror();
						var s = p1;
						p1 = p2;
						p2 = s;
						break;
				}

				var c = op.GetHashCode();
				return p1.GetHashCode() ^ ~p2.GetHashCode() ^ c;
			}

			public override string ToString()
			{
				return string.Format("{0} {1} {2}", Property1, OperatorRepresentation(Operator), Property2);
			}
		}
	}
}