using System;

namespace ModBus
{
    //Класс для чтения значений счетчиков и конвертирования в float, без нужды не трогать 
    internal class PAC3200_Power
    {
        public ushort[] Registers = new ushort[20];
        public float[] Values = new float[5];

        public void ConvertValues()
        {
            Values[0] = CouterConvert(Registers[1], Registers[0]);
            Values[1] = CouterConvert(Registers[5], Registers[4]);
            Values[2] = CouterConvert(Registers[9], Registers[8]);
            Values[3] = CouterConvert(Registers[13], Registers[12]);
            Values[4] = CouterConvert(Registers[17], Registers[16]);
        }

        private float CouterConvert(ushort a, ushort b)
        {
            byte[] x = new byte[2];
            byte[] y = new byte[2];
            byte[] z = new byte[4];

            x = BitConverter.GetBytes(a);
            y = BitConverter.GetBytes(b);
            z = new byte[x.Length + y.Length];
            x.CopyTo(z, 0);
            y.CopyTo(z, x.Length);
            return BitConverter.ToSingle(z, 0);
        }
    }
}