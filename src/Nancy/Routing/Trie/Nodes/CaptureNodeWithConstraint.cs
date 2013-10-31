namespace Nancy.Routing.Trie.Nodes
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A node for constraint captures e.g. {foo:alpha}, {foo:datetime}
    /// </summary>
    public class CaptureNodeWithConstraint : TrieNode
    {
        private string parameterName;
        private string constraint;

        /// <summary>
        /// Score for this node
        /// </summary>
        public override int Score
        {
            get { return 1000; }
        }

        public CaptureNodeWithConstraint(TrieNode parent, string segment, ITrieNodeFactory nodeFactory)
            : base(parent, segment, nodeFactory)
        {
            this.ExtractParameterName();
        }

        /// <summary>
        /// Matches the segment for a requested route
        /// </summary>
        /// <param name="segment">Segment string</param>
        /// <returns>A <see cref="SegmentMatch"/> instance representing the result of the match</returns>
        public override SegmentMatch Match(string segment)
        {
            switch (this.constraint)
            {
                case "alpha":
                    return MatchAlphaConstraint(segment);
                case "bool":
                    return MatchBoolConstraint(segment);
                case "datetime":
                    return MatchDateTimeConstraint(segment);
                case "decimal":
                    return MatchDecimalConstraint(segment);
                case "guid":
                    return MatchGuidConstraint(segment);
                case "int":
                    return MatchIntConstraint(segment);
                default:
                    return this.MatchVariableConstraints(segment);
            }
        }

        private SegmentMatch MatchVariableConstraints(string segment)
        {
            if (this.constraint.Contains('(') && this.constraint.Contains(')'))
            {
                var constraintSplit = this.constraint.Split('(');
                var constraintPart = constraintSplit[0];
                var variablePart = constraintSplit[1].TrimEnd(')');

                switch (constraintPart)
                {
                    case "length":
                        return MatchLengthConstraint(segment, variablePart);
                    case "max":
                        return MatchMaxConstraint(segment, variablePart);
                    case "maxlength":
                        return MatchMaxLengthConstraint(segment, variablePart);
                    case "min":
                        return MatchMinConstraint(segment, variablePart);
                    case "minlength":
                        return MatchMinLengthConstraint(segment, variablePart);
                    case "range":
                        return MatchRangeConstraint(segment, variablePart);
                    default:
                        return SegmentMatch.NoMatch;
                }
            }

            return SegmentMatch.NoMatch;
        }

        private SegmentMatch MatchIntConstraint(string segment)
        {
            long intValue;

            if (!long.TryParse(segment, NumberStyles.Integer, CultureInfo.InvariantCulture, out intValue))
            {
                return SegmentMatch.NoMatch;
            }

            return this.CreateMatch(intValue);
        }

        private SegmentMatch MatchGuidConstraint(string segment)
        {
            if (!Regex.IsMatch(segment, @"\b[A-F0-9]{8}(?:-[A-F0-9]{4}){3}-[A-F0-9]{12}\b", RegexOptions.IgnoreCase))
            {
                return SegmentMatch.NoMatch;
            }

            return this.CreateMatch(segment);
        }

        private SegmentMatch MatchDecimalConstraint(string segment)
        {
            decimal decimalValue;

            if (!decimal.TryParse(segment, NumberStyles.Number, CultureInfo.InvariantCulture, out decimalValue))
            {
                return SegmentMatch.NoMatch;
            }

            return this.CreateMatch(decimalValue);
        }

        private SegmentMatch MatchDateTimeConstraint(string segment)
        {
            DateTime dateTimeValue;

            if (!DateTime.TryParse(segment, out dateTimeValue))
            {
                return SegmentMatch.NoMatch;
            }

            return this.CreateMatch(dateTimeValue);
        }

        private SegmentMatch MatchBoolConstraint(string segment)
        {
            bool boolValue;

            if (!bool.TryParse(segment, out boolValue))
            {
                return SegmentMatch.NoMatch;
            }

            return this.CreateMatch(boolValue);
        }

        private SegmentMatch MatchAlphaConstraint(string segment)
        {
            if (!segment.All(char.IsLetter))
            {
                return SegmentMatch.NoMatch;
            }

            return this.CreateMatch(segment);
        }

        private SegmentMatch MatchRangeConstraint(string segment, string variablePart)
        {
            int minRange;
            int maxRange;
            int intValue;

            var rangeSplit = variablePart.Split(',');

            if (rangeSplit.Length == 2)
            {
                if (!this.TryParseInt(rangeSplit[0], out minRange) ||
                    !this.TryParseInt(rangeSplit[1], out maxRange) ||
                    !this.TryParseInt(segment, out intValue))
                {
                    return SegmentMatch.NoMatch;
                }
            }
            else
            {
                return SegmentMatch.NoMatch;
            }

            if (intValue < minRange || intValue > maxRange)
            {
                return SegmentMatch.NoMatch;
            }

            return this.CreateMatch(intValue);
        }

        private SegmentMatch MatchMinLengthConstraint(string segment, string variablePart)
        {
            int minLength;

            if (!this.TryParseInt(variablePart, out minLength))
            {
                return SegmentMatch.NoMatch;
            }

            if (segment.Length < minLength)
            {
                return SegmentMatch.NoMatch;
            }

            return this.CreateMatch(segment);
        }

        private SegmentMatch MatchMaxLengthConstraint(string segment, string variablePart)
        {
            int maxLength;

            if (!this.TryParseInt(variablePart, out maxLength))
            {
                return SegmentMatch.NoMatch;
            }

            if (segment.Length > maxLength)
            {
                return SegmentMatch.NoMatch;
            }

            return this.CreateMatch(segment);
        }

        private SegmentMatch MatchMinConstraint(string segment, string variablePart)
        {
            int minValue;
            int intValue;

            var rangeSplit = variablePart.Split(',');

            if (!this.TryParseInt(rangeSplit[0], out minValue) ||
                !this.TryParseInt(segment, out intValue))
            {
                return SegmentMatch.NoMatch;
            }
            
            if (intValue < minValue)
            {
                return SegmentMatch.NoMatch;
            }

            return this.CreateMatch(intValue);
        }

        private SegmentMatch MatchMaxConstraint(string segment, string variablePart)
        {
            int minValue;
            int intValue;

            var rangeSplit = variablePart.Split(',');

            if (!this.TryParseInt(rangeSplit[0], out minValue) ||
                !this.TryParseInt(segment, out intValue))
            {
                return SegmentMatch.NoMatch;
            }
            
            if (intValue > minValue)
            {
                return SegmentMatch.NoMatch;
            }

            return this.CreateMatch(segment);
        }

        private SegmentMatch MatchLengthConstraint(string segment, string variablePart)
        {
            int minLength;
            int maxLength;

            var lengthSplit = variablePart.Split(',');

            if (lengthSplit.Length == 2)
            {
                if (!this.TryParseInt(lengthSplit[0], out minLength) ||
                    !this.TryParseInt(lengthSplit[1], out maxLength))
                {
                    return SegmentMatch.NoMatch;
                }
            }
            else if (lengthSplit.Length == 1)
            {
                minLength = 0;

                if (!this.TryParseInt(lengthSplit[0], out maxLength))
                {
                    return SegmentMatch.NoMatch;
                }
            }
            else
            {
                return SegmentMatch.NoMatch;
            }

            if (segment.Length < minLength || segment.Length > maxLength)
            {
                return SegmentMatch.NoMatch;
            }

            return this.CreateMatch(segment);
        }

        private SegmentMatch CreateMatch(object value)
        {
            var match = new SegmentMatch(true);
            match.CapturedParameters.Add(this.parameterName, value);
            return match;
        }

        private bool TryParseInt(string s, out int result)
        {
            return int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out result);
        }

        private void ExtractParameterName()
        {
            var segmentSplit = this.RouteDefinitionSegment.Trim('{', '}').Split(':');

            this.parameterName = segmentSplit[0];
            this.constraint = segmentSplit[1].ToLowerInvariant();
        }
    }
}
