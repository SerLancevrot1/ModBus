using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModBus.Classes
{
    internal class Gas_MongoNode
    {
        public ObjectId _id { get; set; }
        public int ID { get; set; }
        public string name { get; set; }
        public float value { get; set; }
        public DateTime dateTime { get; set; }
    }
}
