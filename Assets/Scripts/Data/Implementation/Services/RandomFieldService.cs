using Game.Data.Fields;

namespace Game.Data.Services
{
    public class RandomFieldService<T> where T : IDataField
    {
        protected T GetRandomField(T dataField)
        {
            if (dataField is RandomField<T> randomListField)
            {
                float totalChance = 0f;
                foreach (var randomData in randomListField.Value)
                {
                    totalChance += randomData.Chance;
                }

                float randomValue = UnityEngine.Random.Range(0f, totalChance);

                float cumulativeChance = 0f;
                
                foreach (var rewardRandomData in randomListField.Value)
                {
                    cumulativeChance += rewardRandomData.Chance;
                    if (randomValue <= cumulativeChance)
                    {
                        return rewardRandomData.Field;
                    }
                }
            }

            return default;
        }
    }
}