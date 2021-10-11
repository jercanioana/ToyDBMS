using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBMSServer
{
    class MongoDBAcess
    {
        private string DatabaseName;
        private IMongoDatabase MongoDatabase;

        public MongoDBAcess(string _databaseName)
        {
            DatabaseName = _databaseName;
            CreateDBConnection();
        }

        // Note: There is no MongoDB method to create a database; instead, it is implicitly created when a collection is added to it 
        // => we don't need an explicit method to create a database in MongoDB when one is created in the XML file, as it will be
        // created when a table is added to the database in the XML 
        private void CreateDBConnection()
        {
            try
            {
                var mongoClient = new MongoClient("mongodb+srv://mongo_user:parolaMongo@cluster0.qsvie.mongodb.net/" + DatabaseName + "?retryWrites=true&w=majority");
                MongoDatabase = mongoClient.GetDatabase(DatabaseName);
            }
            catch (Exception)
            {
                throw new Exception("Could not create MongoDB connection");
            }
        }

        public void CreateCollection(string collectionName)
        {
            try
            {
                MongoDatabase.CreateCollection(collectionName);
            }
            catch
            {
                // Should never happen, since duplicate table validation is already done baed on the XML file 
                throw new Exception("Could not create MongoDB Collection: " + collectionName);
            }
        }

        public void InsertKVIntoCollection(string collectionName, string key, string value)
        {
            try
            {
                var mongoCollection = MongoDatabase.GetCollection<BsonDocument>(collectionName);
                BsonDocument newRecord = new BsonDocument().Add("_id", key).Add("value", value);
                mongoCollection.InsertOne(newRecord);
            }
            catch (Exception)
            {
                throw new Exception("Duplicate Key:" + key);
            }
        }

        public void RemoveValueFromCollection(string collectionName, string value)
        {
            try
            {
                var mongoCollection = MongoDatabase.GetCollection<BsonDocument>(collectionName);
                var allRecords = GetEntireCollection(collectionName);
                foreach (var record in allRecords)
                {
                    var recordValues = record.GetElement("value").Value.ToString().Split('#');
                    var newRecordValue = "";
                    foreach (var recordValue in recordValues)
                    {
                        if (recordValue != value)
                        {
                            newRecordValue += recordValue + '#';
                        }
                    }

                    if (newRecordValue != "")
                    {
                        newRecordValue = newRecordValue.Remove(newRecordValue.Length - 1);
                    }

                    if (newRecordValue != record.GetElement("value").Value.ToString())
                    {
                        if (newRecordValue == "")
                        {
                            RemoveKVFromCollection(collectionName, record.GetElement("_id").Value.ToString());
                        }
                        else
                        {
                            var filter = Builders<BsonDocument>.Filter.Eq("_id", record.GetElement("_id").Value);
                            var update = Builders<BsonDocument>.Update.Set("value", newRecordValue);
                            mongoCollection.UpdateOne(filter, update);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Could not remove Value: " + value + " from MongoDB Collection: " + collectionName);
            }
        }

        public void RemoveByValueFromCollection(string collectionName, string value)
        {
            try
            {
                var mongoCollection = MongoDatabase.GetCollection<BsonDocument>(collectionName);
                var allRecords = GetEntireCollection(collectionName);
                foreach (var record in allRecords)
                {
                    var recordValue = record.GetElement("value").Value.ToString();
                    if (recordValue == value)
                    {
                        RemoveKVFromCollection(collectionName, record.GetElement("_id").Value.ToString());
                    }
                }
            }
            catch (Exception)
            {
                throw new Exception("Could not remove Value: " + value + " from MongoDB Collection: " + collectionName);
            }
        }

        public void RemoveKVFromCollection(string collectionName, string key)
        {
            try
            {
                var mongoCollection = MongoDatabase.GetCollection<BsonDocument>(collectionName);
                var deleteFilter = Builders<BsonDocument>.Filter.Eq("_id", key);
                mongoCollection.DeleteOne(deleteFilter);
            }
            catch (Exception)
            {
                throw new Exception("Could not delete the entry from MongoDB Collection: " + collectionName + "with Key: " + key);
            }
        }

        public void RemoveAllKVFromCollection(string collectionName)
        {
            try
            {
                var mongoCollection = MongoDatabase.GetCollection<BsonDocument>(collectionName);
                var deleteFilter = Builders<BsonDocument>.Filter.Empty;
                mongoCollection.DeleteMany(deleteFilter);
                MongoDatabase.DropCollection(collectionName);
            }
            catch (Exception)
            {
                throw new Exception("Could not delete all entries from MongoDB Collection: " + collectionName);
            }
        }

        public static void RemoveDatabase(string databaseName)
        {
            try
            {
                var mongoClient = new MongoClient("mongodb+srv://mongo_user:parolaMongo@cluster0.qsvie.mongodb.net/" + databaseName + "?retryWrites=true&w=majority");
                mongoClient.DropDatabase(databaseName);
            }
            catch (Exception)
            {
                throw new Exception("Could not delete MongoDB Database: " + databaseName);
            }
        }

        public void RemoveCollection(string collectioName)
        {
            try
            {
                MongoDatabase.DropCollection(collectioName);
            }
            catch (Exception)
            {
                throw new Exception("Could not remove MongoDB Collection: " + collectioName);
            }
        }

        public List<BsonDocument> GetEntireCollection(string collectionName)
        {
            try
            {
                var mongoCollection = MongoDatabase.GetCollection<BsonDocument>(collectionName);
                return mongoCollection.Find(new BsonDocument()).ToList();
            }
            catch (Exception)
            {
                throw new Exception("Could not retrieve the contents of MongoDB Collection: " + collectionName);
            }
        }

        public List<BsonDocument> GetCollectionFilteredByKey(string collectionName, FilterDefinition<BsonDocument> filter)
        {
            try
            {
                var mongoCollection = MongoDatabase.GetCollection<BsonDocument>(collectionName);
                return mongoCollection.Find(filter).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not filter the MongoDB Collection: " + ex.Message);
            }
        }

        public string GetRecordValueWithKey(string collectionName, string key)
        {
            try
            {
                var mongoCollection = MongoDatabase.GetCollection<BsonDocument>(collectionName);
                var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
                return mongoCollection.Find(filter).First().GetElement("value").Value.ToString();
            }
            catch (Exception)
            {
                throw new Exception("Could not retrieve the record with key " + key + " from MongoDB Collection: " + collectionName);
            }
        }

        public bool CollectionContainsKey(string collectionName, string key)
        {
            try
            {
                var mongoCollection = MongoDatabase.GetCollection<BsonDocument>(collectionName);
                var filter = Builders<BsonDocument>.Filter.Eq("_id", key);
                return mongoCollection.Find(filter).Any();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public bool CollectionHasDocuments(string collectionName)
        {
            try
            {
                return GetEntireCollection(collectionName).Count > 0;
            }
            catch (Exception)
            {
                throw new Exception("Could not retrieve the number of records from MongoDB Collection: " + collectionName);
            }
        }

        public bool CollectionExists(string collectionName)
        {
            try
            {
                var filter = new BsonDocument("name", collectionName);
                var options = new ListCollectionNamesOptions { Filter = filter };

                return MongoDatabase.ListCollectionNames(options).Any();
            }
            catch (Exception)
            {
                throw new Exception("Could not check that MongoDB Collection: " + collectionName + " exists");
            }
        }
    }
}
