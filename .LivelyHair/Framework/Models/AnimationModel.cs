﻿using LivelyHair.Framework.Models.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LivelyHair.Framework.Models
{
    public class AnimationModel
    {
        internal enum Type
        {
            Unknown,
            Idle,
            Moving
        }

        public int Frame { get; set; }
        public bool OverrideStartingIndex { get; set; }
        public List<Condition> Conditions { get; set; } = new();
        public int Duration { get; set; } = 1000;

        internal bool HasCondition(Condition.Type type)
        {
            return Conditions.Any(c => c.Name == type);
        }

        internal Condition GetConditionByType(Condition.Type type)
        {
            return Conditions.FirstOrDefault(c => c.Name == type);
        }
    }
}
