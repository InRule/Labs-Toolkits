# Toolkits

There are several good scenarios for re-using part of one rule application in another.  While we support sharing of schemas and rulesets using the catalog, We wanted to explore doing the same from the file system.  These cases are especially useful when customers don't use the catalog and rely on source control and custom solutions for their lifecycle.

When you import from another rule application, you are breaking the lifecycle from the source.  This is fine if you want to continue being the owner; however, lot's of folks want another team to maintain the dependencies.  This project helps folks manage the lifecycle between their content and imported content.  The advantage of the toolkit is it's removal and replacement by newer versions, etc. 

In time, this will grow into an installable extension.  
  


### Prerequisites

If the developer modifies any of the code, he/she should be familiar with the following tools and frameworks:

Visual Studio 2013 or better

irAuthor 5.x (installed)




### Testing

Just run the NUnit tests. 



## How It Works



**Import**

Helper h = new Helper();

RuleApplicationDef source = RuleApplicationDef.Load(_sourcePath);

RuleApplicationDef dest = RuleApplicationDef.Load(_destPath);

h.ImportToolkit(source, dest);

 

**Remove**

h.RemoveToolkit(source,dest);


During the import, the source rule application is imported and preserves all of the GUIDs from the original.  We stamp each artifact with a custom attribute that describes the version, GUID and name of the source rule application.  Finally, the source rule application is encoded and stored in the destination ruleapplication for safe keeping.  Down the road it might be useful for checks and reparing.

After each import, the rule application is validated to ensure it's still working. 


## Limitations

1) Importing a rule application as a toolkit brings in all artifacts.  You cannot use the utility for merging content (yet).
2) Once a rule application has been imported as a toolkit, it's not obvious to the author what belongs to a toolkit.  More work needs to be done to create visual decoration of these dependencies.  I am working on a concept to use Catorgories for just such a problem.
3) Imported artifacts can still be modified (they remain mutable).  
4) If you are using the Catalog, performance will decline as the rule application continues to grow in size.  Future implementations will likely focus on this problem and omit the local storage of the source ruleapplication.



# Merge By Category

Sometimes breaking up rule applications into toolkits is not enough to keep everyone working.  You might have several team members pound away at a rule app during the day and they are all in the same Rule Set.  In these cases, you can share a base rule application between them and then tag each rule with a Category.

The Helper now includes methods that import and merge rules (and other artifacts) into a rule application.  After a merge, the rule author in charge of the merge should make sure the rules were placed in expected locations.  There are cases, where rules might be placed somewhere un-expected and the execution order will be effected.

**Specialized Import Using Category**

Helper h = new Helper();

RuleApplicationDef source = RuleApplicationDef.Load(sourcePath);

RuleApplicationDef dest = RuleApplicationDef.Load(destPath);

//When category is included, the method attempts a merge

h.ImportRuleApp(source, dest, "Category1");





