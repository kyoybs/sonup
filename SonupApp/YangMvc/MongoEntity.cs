//using MongoDB.Bson.Serialization.Attributes;
//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace YangMvc
//{
//    public class MongoEntity
//    {
//        public virtual string _id { get; set; }
//    }

//    public class ImeiEntity : MongoEntity
//    {
//        public string Sn { get; set; }

//        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
//        public DateTime CreateTime { get; set; }

//        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
//        public DateTime? UpdateTime { get; set; }
//    }
//}
