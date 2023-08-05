using RegionExtension.RegionTriggers.Conditions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RegionExtension.RegionTriggers.RegionProperties
{
    public class ConditionDataPair<T> : IEnumerable<T>
    {
        public T this[int i] { get => Data[i]; set => Data[i] = value; }
        public ConditionDataPair(List<IRegionCondition> conditions, List<T> data)
        {
            Conditions = conditions;
            Data = data;
        }

        public List<IRegionCondition> Conditions { get; set; }
        public List<T> Data { get; set; }

        public IEnumerator<T> GetEnumerator()
        {
            foreach(T i in Data)
                yield return i;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (T i in Data)
                yield return i;
        }
        public bool ConditionsEqual(IEnumerable<IRegionCondition> regionConditions)
        {
            if (Conditions.Count != regionConditions.Count())
                return false;
            return Conditions.All(c => regionConditions.Any(tc => tc.GetNames()[0].Equals(c.GetNames()[0])));
        }

        public ConditionStringPair ConvertToString() =>
            new ConditionStringPair(Conditions.GenerateConditionsString(), string.Join(' ', Data.Select(x => x.ToString())));
        public static ConditionDataPair<T> GetFromString(ConditionStringPair stringPair) =>
            new ConditionDataPair<T>(ConditionManager.GetRegionConditionsFromString(stringPair.Conditions).ToList(), stringPair.Args.Split(' ').Select(s => (T)Convert.ChangeType(s, typeof(T))).ToList());
    }
}
