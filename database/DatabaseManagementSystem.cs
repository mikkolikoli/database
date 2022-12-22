using System;
namespace database;

public class DatabaseManagementSystem
{
    private Dictionary<string, Database> databases;

    public DatabaseManagementSystem()
	{
			databases = new Dictionary<string, Database>();
	}

	public void CreateDatabase(string name)
	{
		if (databases.ContainsKey(name))
		{
			// TODO: create exception
			throw new NotImplementedException();
		}

		Database database = new(name);
		databases.Add(name, database);
	}

	public void CreateCollection(string databaseName, string collectionName)
	{
		if (!databases.ContainsKey(databaseName))
		{
			// TODO: maybe custom exception?
			throw new KeyNotFoundException("Database does not exist.");
		}

		databases[databaseName].CreateCollection(collectionName);
	}

	public string ReadData(string databaseName, string collectionName, int id)
	{
		if (!databases.ContainsKey(databaseName))
		{
			throw new KeyNotFoundException("Database does not exist.");
		}

		// TODO: add read cache

		return databases[databaseName].GetData(collectionName, id);
	}

	public int WriteData(string databaseName, string collectionName, string data)
	{
		if (!databases.ContainsKey(databaseName))
		{
			throw new KeyNotFoundException("Database does not exist.");
		}

		return databases[databaseName].AddData(collectionName, data);
	}

	public string UpdateData(string databaseName, string collectionName, int id, string data)
	{
		if (!databases.ContainsKey(databaseName))
		{
			throw new KeyNotFoundException("Database does not exist.");
		}

		return databases[databaseName].UpdateData(collectionName, id, data);
	}

	public string[] GetDatabases()
	{
		return databases.Keys.ToArray();
	}

	public string[] GetCollections(string databaseName)
	{
		return databases[databaseName].Collections.ToArray();
	}

	public string[] GetAllFromCollection(string databaseName, string collectionName)
	{
		return databases[databaseName].GetAllItems(collectionName);
	}
}

