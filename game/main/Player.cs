using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wisetsar
{
    public class Player
    {
        public long Piety { get; set; } = 100;
        public long CurrentStage { get; set; }
        public DateTime PlayedDate { get; set; }

        public void Write(BinaryWriter bw)
        {
            bw.Write(Piety);
            bw.Write(CurrentStage);
            bw.Write(PlayedDate.Ticks);
        }

        public static Player Read(BinaryReader br)
        {
            var ret = new Player();
            ret.Piety = br.ReadInt64();
            ret.CurrentStage = br.ReadInt64();
            ret.PlayedDate = new DateTime(br.ReadInt64());
            return ret;
        }

        public static Player[] ReadPlayers(BinaryReader br, long count)
        {
            var bytes = br.ReadBytes((int)count * 24);
            var ret = new Player[count];
            for (var i = 0; i < count; i++)
            {
                ret[i] = new Player();
                ret[i].Piety = BitConverter.ToInt64(bytes, i * 24);
                ret[i].CurrentStage = BitConverter.ToInt64(bytes, i * 24 + 8);
                ret[i].PlayedDate = new DateTime(BitConverter.ToInt64(bytes, i * 24 + 16));
            }
            return ret;
        }
    }
}
