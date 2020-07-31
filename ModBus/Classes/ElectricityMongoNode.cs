using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace ModBus.Classes
{
    internal class ElectricityMongoNode
    {
        public ObjectId _id { get; set; }
        public int ID { get; set; }
        public DateTime dateTime { get; set; }
        public float wP_in { get; set; }
        public float WP_out { get; set; }
        public float WQ_in { get; set; }
        public float WQ_oup { get; set; }
        public float WQ { get; set; }
}
