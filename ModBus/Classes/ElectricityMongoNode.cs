using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModBus.Classes
{
    internal class ElectricityMongoNode
    {
        public ObjectId _id { get; set; } // ID для БД, нужно для корректной работы
        public int ID { get; set; } // ID в самой БД
        public DateTime dateTime { get; set; } // время записи
        public float wP_in { get; set; } 
        public float WP_out { get; set; }
        public float WQ_in { get; set; }  // 5 значений счетчиков 
        public float WQ_oup { get; set; }
        public float WQ { get; set; }
    }
}
