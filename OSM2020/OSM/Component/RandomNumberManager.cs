using MathNet.Numerics.Distributions;
using MathNet.Numerics.Random;
using OSM2020.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OSM2020.OSM.Component
{
    class RandomNumberManager
    {
        Dictionary<SeedEnum, ExtendRandom> RandomDictionary;

        public RandomNumberManager()
        {
            this.RandomDictionary = new Dictionary<SeedEnum, ExtendRandom>();
        }

        public void Register(SeedEnum seed_enum, int seed)
        {
            ExtendRandom extended_random = new ExtendRandom(seed_enum, seed);
            this.RandomDictionary[seed_enum] = extended_random;
        }

        public ExtendRandom Get(SeedEnum seed_enum)
        {
            if (!this.RandomDictionary.ContainsKey(seed_enum))
            {
                throw new Exception(seed_enum.ToString() + " is not registered");
            }
            return this.RandomDictionary[seed_enum];
        }

        public void SetAgentGenerateRand(int seed)
        {
            this.Register(SeedEnum.AgentGenerateSeed, seed);
        }

        public ExtendRandom GetAgentGenerateRand()
        {
            return this.Get(SeedEnum.AgentGenerateSeed);
        }
    }
}
