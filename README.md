# PropertiesManager

This PropertiesManager library provides methods to handle Java-like config/properties files. It also supports maintaining layouts and comments.

_This library was built and tested on .NET Framework 2.0._

### Installation

The package is available on Nuget: [Quellatalo.Nin.PropertiesManager](https://www.nuget.org/packages/Quellatalo.Nin.PropertiesManager/)

## Example code

```cs
public void PropManagerTest()
{
    // Creates a sample config.prop file with 1 comment line, and one property
    PropFileManager pm = new PropFileManager("config.prop");

    // Skipped pm.Load(); method, it means we are going to create a whole new file

    // Adds some contents
    pm.AddLineEntry("\t# modified count");
    pm.AddLineEntry("\tcount = 0"); // pm["\tcount"] = "0"; // this works too

    // Check the contents
    Console.WriteLine(pm[0]); // print out the comment
    Console.WriteLine(pm["count"]); // print out the property's value
    Console.WriteLine(pm.GetFullText()); // print out what will be saved to file
    pm.Save(); // Saves to file

    // ...

    string modifiedTimeKey = "modified time";
    PropFileManager p = new PropFileManager("config.prop");
    p.Load(); // Loads the existing data on the file

    // Updates the count property
    p.SetProperty("count", p.GetInt("count") + 1); // count is now 1
    // Same as:
    p["count"] = (p.GetInt("count") + 1).ToString(); // count is now 2

    // Adds a new property
    if (!p.HasProperty(modifiedTimeKey)) // Checks if the property is not already exist
    {
        p.AddLineEntry(); // Adds a new empty line
        p.AddLineEntry("##### Log #####"); // Adds a comment line
        p.AddLineEntry(); // Adds another new empty line
        p.AddLineEntry("# Last modified"); // Adds a comment line
    }
    p.SetProperty(modifiedTimeKey, DateTime.Now); // Sets the property
    p.InsertLineEntry(); // Inserts an empty line to the beginning
    p.InsertLineEntry(4, "##### LOG #####"); // Inserts a comment line just about the ##### Log ##### line
    p.RemoveLineEntryAt(6); // Removes the empty line after ##### Log ##### line
    p.SetLineEntry(4, "##### LOGGING #####"); // Modifies the comment line ##### LOG #####
    Console.WriteLine(p.GetDateTime(modifiedTimeKey)); // Prints out the modified time
    p.Save(); // Saves to file
}
```


License
----

MIT


**It's free. El Psy Congroo!**
