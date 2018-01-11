//using MongoDB.Bson;
//using MongoDB.Driver;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Text;
//using System.Threading.Tasks;

//namespace YangMvc
//{
//    public class MongoHelper
//    {
//        private static ConcurrentDictionary<string, MongoHelper> Mongos = new ConcurrentDictionary<string, MongoHelper>();

//        private MongoClient _Client;
//        private MongoClient Client
//        {
//            get
//            {
//                if (_Client == null)
//                {
//                    //MongoClientSettings s = MongoClientSettings.FromUrl(new MongoDB.Driver.MongoUrl(MongoUrl));
//                    //s.Credentials
//                    _Client = new MongoDB.Driver.MongoClient(MongoUrl);
//                }
//                return _Client;
//            }
//        }

//        /// <summary>
//        /// 批量查询示例
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="sn"></param>
//        /// <param name="top"></param>
//        /// <param name="dbName"></param>
//        /// <param name="collectionName"></param>
//        /// <returns></returns>
//        public List<T> GetList<T>(string sn, int top, MongoDBs dbName, GpsDbCollections collectionName) where T : ImeiEntity
//        {
//            var collection = GetCollection<T>(dbName, collectionName);
//            var rows = collection.Find(gd => gd.Sn == sn)
//                .SortByDescending(gd => gd.UpdateTime)
//                .Limit(top).ToList();
//            return rows;
//        }

//        /// <summary>
//        /// 批量插入示例
//        /// </summary>
//        /// <param name="deviceDatas"></param>
//        public async Task AddAsync<T>(List<T> entitys, MongoDBs dbName, GpsDbCollections collectionName) where T : ImeiEntity
//        {
//            if (entitys.Count == 0)
//                return;
//            foreach (ImeiEntity item in entitys)
//            {
//                if (string.IsNullOrEmpty(item._id))
//                {
//                    item._id = ObjectId.GenerateNewId().ToString();
//                }
//            }
//            IMongoCollection<T> collection = GetCollection<T>(dbName, collectionName);
//            await collection.InsertManyAsync(entitys);
//        }

//        /// <summary>
//        /// 批量更新示例, 仅用于示例
//        /// </summary>
//        /// <param name="deviceDatas"></param>
//        public async Task UpdateAsync<T>(List<T> newDatas, MongoDBs dbName, GpsDbCollections collectionName) where T : MongoEntity
//        {
//            if (newDatas.Count == 0)
//                return;

//            List<WriteModel<T>> requests = new List<WriteModel<T>>();

//            foreach (var item in newDatas)
//            {
//                var filter = Builders<T>.Filter.Eq(gd => gd._id, item._id);
//                var update = Builders<T>.Update.Set(gd => gd._id, item._id);
//                UpdateOneModel<T> uom = new UpdateOneModel<T>(filter, update);
//                uom.IsUpsert = true;
//                requests.Add(uom);
//            }

//            IMongoCollection<T> collection = GetCollection<T>(dbName, collectionName);
//            await collection.BulkWriteAsync(requests);

//        }

//        public IMongoCollection<T> GetCollection<T>(MongoDBs dbName, GpsDbCollections collectionName)
//        {
//            var db = Client.GetDatabase(dbName.ToString());
//            var collection = db.GetCollection<T>(collectionName.ToString());
//            return collection;
//        }

//        public void DeleteCollection(MongoDBs dbName, string collectionName)
//        {
//            var db = Client.GetDatabase(dbName.ToString());
//            db.DropCollection(collectionName.ToString());
//        }

//        public IMongoCollection<T> GetCollection<T>(string dbName, string collectionName)
//        {
//            var db = Client.GetDatabase(dbName);
//            var collection = db.GetCollection<T>(collectionName);
//            return collection;
//        }

//        public IMongoCollection<BsonDocument> GetCollection(string dbName, string collectionName)
//        {
//            var db = Client.GetDatabase(dbName);
//            var collection = db.GetCollection<BsonDocument>(collectionName);
//            return collection;
//        }

//        public static MongoHelper Create(string mongoServerKey = "MainMongo")
//        {
//            if (Mongos.TryGetValue(mongoServerKey, out MongoHelper mongo))
//            {
//                return mongo;
//            }

//            MongoHelper mh = new MongoHelper();
//            mh.MongoUrl = "mongodb://" + Config.Get(mongoServerKey);

//            //mongodb://[username:password@]host1[:port1][,host2[:port2],...[,hostN[:portN]]][/[database][?options]]
//            //            mh.MongoUrl = @"mongodb://root:Mayi2017@dds-bp122b2679954d542.mongodb.rds.aliyuncs.com:3717,
//            //dds-bp122b2679954d541.mongodb.rds.aliyuncs.com:3717?replicaSet=mgset-3125599";

//            Mongos.TryAdd(mongoServerKey, mh);
//            return mh;
//        }

//        private MongoHelper()
//        {
//        }

//        private string __MongoUrl;

//        private string MongoUrl { get; set; }

//        internal void ResetDB(string dbName, string collectionName)
//        {
//            //var col = GetCollection(dbName , collectionName);
//            //col.DeleteMany(FilterDefinition<GpsData>.Empty);
//        }

//        public List<string> ListCollections(string dbName)
//        {
//            var db = Client.GetDatabase(dbName);
//            var cols = db.ListCollections();

//            List<string> colNames = new List<string>();
//            foreach (var item in cols.ToList())
//            {
//                string collectionName = item["name"].ToString();
//                colNames.Add(collectionName);
//            }
//            return colNames;
//        }


//        public List<string> ListDBs()
//        {
//            var dbs = Client.ListDatabases();
//            List<string> colNames = new List<string>();
//            foreach (var item in dbs.ToList())
//            {
//                string collectionName = item["name"].ToString();
//                colNames.Add(collectionName);
//            }
//            return colNames;
//        }

//        public long GetCount(MongoDBs mongoDB, GpsDbCollections collectionName)
//        {
//            IMongoCollection<BsonDocument> collection = this.GetCollection<BsonDocument>(mongoDB, collectionName);
//            return collection.Count(FilterDefinition<BsonDocument>.Empty);
//        }

//        public long GetCount(string mongoDB, string collectionName)
//        {
//            IMongoCollection<BsonDocument> collection = this.GetCollection<BsonDocument>(mongoDB, collectionName);
//            return collection.Count(FilterDefinition<BsonDocument>.Empty);
//        }

//        public void InitDB()
//        {
//            //MongoClient client = new MongoDB.Driver.MongoClient(MongoUrl);
//            //var db = client.GetDatabase("GpsDB"); 
//            //var collection = db.GetCollection<GpsData>("GpsData");
//            //if(collection == null)
//            //{
//            //    db.CreateCollection("GpsData");
//            //    collection = db.GetCollection<GpsData>("GpsData");
//            //} 
//        }


//    }
//}
