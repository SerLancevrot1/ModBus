using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModBus.Classes
{
    class ProductionLine
    {
        public ObjectId _id { get; set; } // ID для БД, нужно для корректной работы
        public int ID { get; set; } // ID в самой БД
        public DateTime dateTime { get; set; } // время записи
        public bool IsWork { get; set; }
    }
}
