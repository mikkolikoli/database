namespace database;

public class DatabaseTable
{
    public string Id => id;

    public Dictionary<string, ElementTypes> Schema => schema;
    public string Key => key;
    public string Collection => collection;

    private readonly string[] keys;

    private readonly bool noScriptTags;

    private readonly string id;
    private readonly Dictionary<string, ElementTypes> schema;
    private readonly string key;
    private readonly string collection;

    private readonly List<string> dataKeys;

    private readonly StreamWriter file;
    private readonly string filePath;

    public DatabaseTable(string id,
                         string collection,
                         Dictionary<string, ElementTypes> schema,
                         string key,
                         bool noScriptTags = true)
    {
        this.id = id;
        this.schema = schema;
        this.key = key;
        this.noScriptTags = noScriptTags;
        this.collection = collection;

        keys = schema.Keys.ToArray();
        dataKeys = new List<string>();

        filePath = $"./database/{collection}/{id}.csv";
        file = new StreamWriter(filePath,
                                append: true);

        ValidateSchema();
    }

    /// <summary>
    /// Validates the schema. Checks if there are enough columns, the table key
    /// is included, if there are no duplicate keys and if the keys don't
    /// contain anything, that is not a letter
    /// </summary>
    /// <exception cref="ApplicationException">
    /// Throws exception if schema is not valid
    /// </exception>
    private void ValidateSchema()
    {
        if (schema.Count <= 1)
        {
            throw new ApplicationException("Not enough columns");
        }
        else if (!schema.ContainsKey(key))
        {
            throw new ApplicationException("key is not in schema");
        }

        // check for duplicate keys
        if (keys.Length != keys.Distinct().Count())
        {
            throw new ApplicationException("Duplicate keys");
        }

        foreach (string schemaKey in schema.Keys)
        {
            if (!schemaKey.All(char.IsLetter))
            {
                throw new ApplicationException("Key must only contain letters");
            }
        }
    }

    /// <summary>
    /// writes a dataset to the file. Checks also if the data is valid
    /// </summary>
    /// <param name="data">array of strings, the data to be written</param>
    public async void WriteData(string[] data)
    {
        ValidateData(data);

        string line = string.Join(";", data);
        await file.WriteLineAsync(line);
    }

    /// <summary>
    /// Reads the table and returns the item if it exists
    /// </summary>
    /// <param name="id">string, the id of the data entry</param>
    /// <returns>The data set if it exists, an empty array if not</returns>
    public string[] ReadData(string id)
    {
        // check if item exists and return empty array if not
        if (!dataKeys.Contains(id))
        {
            return Array.Empty<string>();
        }

        // get all lines from file
        string[] allData = File.ReadAllLines(filePath);

        // search for correct entry
        foreach (string entry in allData)
        {
            string[] data = entry.Split(';');
            if (data[0] == id)
            {
                return data;
            }
        }

        // will never reach this, used to satisfy c# compiler
        return Array.Empty<string>();
    }

    /// <summary>
    /// Method validates the data from the user and checks if it fits the schema,
    /// if the key of the data entry is unique, if the characters are valid and
    /// if it has script tags (only if noScriptTags is true)
    /// </summary>
    /// <param name="data">array of strings for the data</param>
    /// <exception cref="InvalidDataException">Throws exception
    /// if the data isn't valid</exception>
    private void ValidateData(string[] data)
    {
        if (data.Length != schema.Count)
        {
            throw new InvalidDataException("Doesn't fit schema");
        }

        if (data[0] == "")
        {
            throw new InvalidDataException("Id is empty");
        }

        // check if key is unique
        if (dataKeys.Contains(data[0]))
        {
            throw new InvalidDataException("Id is not unique");
        }

        // goes through the entire data validates each individual data entry
        for (int i = 0; i < data.Length; i++)
        {
            if (noScriptTags &&
                (data[i].Contains("<script>") || data[i].Contains("</script>")))
            {
                throw new InvalidDataException("Item has script tags");
            }

            if (!CheckType(data[i], i))
            {
                throw new InvalidDataException("Item is of wrong type");
            }

            if (data[i] == "")
            {
                throw new InvalidDataException("Item is empty");
            }
        }

        // check if data is invalid (has semicolon)
        if (data.Any(item => item.Contains(';')))
        {
            throw new InvalidDataException("Item contains semicolon");
        }

        // adds the key of the data to the list
        dataKeys.Add(data[0]);
    }

    /// <summary>
    /// checks if the type of the item corresponds with the type in the schema
    /// </summary>
    /// <param name="item">string, the item to be checked</param>
    /// <param name="index">the index of the item in it's own dataset,
    /// used to get the type of the schema</param>
    /// <returns>true if the type of the item is correct,
    /// false if not</returns>
    private bool CheckType(string item, int index)
    {
        string keyOfItem = keys[index];
        ElementTypes type = schema[keyOfItem];

        switch (type)
        {
            case ElementTypes.INTEGER:
                return int.TryParse(item, out _);
            case ElementTypes.STRING:
                return true;
            case ElementTypes.BOOLEAN:
                return bool.TryParse(item, out _);
            default:
                return false;
        }
    }
}

