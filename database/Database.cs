using System;
namespace database;

public class Database
{
    public string Name { get; }
    public List<string> Collections { get; }

		public Database(string name)
		{
			this.Name = name;
		}

    public override bool Equals(object? obj)
    {
        return obj is Database database &&
               Name == database.Name;
    }

    public void CreateCollection(string name)
    {
        return;
    }

    public string GetData(string collection, int id)
    {
        return "";
    }

    public int AddData(string collection, string data)
    {
        return 0;
    }

    public string UpdateData(string collection, int id, string data)
    {
        return "";
    }

    public string[] GetAllItems(string collection)
    {
        return new string[1];
    }
}

