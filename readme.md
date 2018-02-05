# Toolkits

There are several good scenarios for re-using part of one rule application in another.  While we support sharing of schemas and rulesets using the catalog, I wanted to explore doing the same from the file system.  These cases are especially useful when customers don't use the catalog and rely on source control and custom solutions for their lifecycle.

When you import from another rule application, you are breaking the lifecycle from the source.  This is fine if you want to continue being the owner; however, lot's of folks want antoher team to maintain the dependencies.  This project helps folks manage the lifecycle between their content and imported content.  The advantage of the toolkit is it's removal and replacement by newer versions, etc. I have also been working on improved import by category (tags); however, this is not working yet.  

In time, this will grow into an installable extension.  
  


### Prerequisites

If the developer modifies any of the code, he/she should be familiar with the following tools and frameworks:

Visual Studio 2013 or better

irAuthor 5.x (installed)




### Testing

Just run the NUnit tests. 



## How It Works



Import

Helper h = new Helper();

RuleApplicationDef source = RuleApplicationDef.Load(_sourcePath);

RuleApplicationDef dest = RuleApplicationDef.Load(_destPath);

h.ImportToolkit(source, dest);

 

Remove

h.RemoveToolkit(source,dest);



## Limitations

1) Importing a rule application as a toolkit brings in all artifacts.  You cannot use the utility for merging content (yet).
2) Once a rule application has been imported as a toolkit, it's not obvious to the author what belongs to a toolkit.  More work needs to be done to create visual decoration of these dependencies.  I am working on a concept to use Catorgories for just such a problem.
3) Imported artifacts can still be modified (they remain mutable).  








