using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Snake
{
    [Serializable]
    class Scores
    {
        public int[] staticSpeed = new int[10];
        public int dynamicSpeed;

        public void Save(Scores obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream("Scores.dat", FileMode.OpenOrCreate))
                formatter.Serialize(fs, obj);
        }

        public Scores Load()
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream fs = new FileStream("Scores.dat", FileMode.OpenOrCreate))
                return (Scores)formatter.Deserialize(fs);
        }
    }
}
