using System;
using System.Collections.ObjectModel;
using System.Linq;
using InRule.Authoring.Services;
using InRule.Authoring.Windows;
using InRule.Repository;
using InRule.Repository.RuleElements;
using InRule.Authoring.Extensions;

namespace InRule.Labs.Toolkit.Author
{
    public class CategoryModel
    {
        public SelectionManager SelectionManager { get; set; }
        public RuleApplicationController RuleApplicationController { get; set; } 
        public ObservableCollection<CategoryInfo> Categories { get; set; }
        private const string UNCATEGORIZED_CATEGORY = "(Uncategorized)";

        public CategoryModel(RuleApplicationDef ruleAppDef)
        {
            Categories = new ObservableCollection<CategoryInfo>();

            AddUncategorizedRuleSets(ruleAppDef);

            ruleAppDef.Categories.ForEach(c => Categories.Add(new CategoryInfo(c, ruleAppDef)));
        }

        private object _selectedItem;
        public object SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                try
                {
                    var item = value as RuleSetInfo;
                    if (item != null)
                    {
                        _selectedItem = item;
                        SelectionManager.SelectedItem = item.RuleSetDef;
                    }
                }
                catch (Exception ex)
                {
                    MessageBoxFactory.Show("Error trying to set selected item: " + value.ToString() + Environment.NewLine + ex.ToString(), "Error in setting selected item", MessageBoxFactoryImage.Error);
                }
            }
        }

        public void AddCategoryToDef(RuleRepositoryDefBase def, string category)
        {
            var ruleSetDef = def as RuleSetDef;
            if (ruleSetDef != null)
            {
                var categoryInfo = GetCategoryInfo(category, def.ThisRuleApp);

                categoryInfo.RuleSets.Add(new RuleSetInfo(ruleSetDef));

                var uncatInfo = GetCategoryInfo(UNCATEGORIZED_CATEGORY, def.ThisRuleApp);

                uncatInfo.RemoveRuleSetInfo(ruleSetDef);
            }
        }

        public void RemoveCategoryFromDef(RuleRepositoryDefBase def, string category)
        {
            var ruleSetDef = def as RuleSetDef;
            if (ruleSetDef != null)
            {
                var categoryInfo = GetCategoryInfo(category, def.ThisRuleApp, false);

                if (categoryInfo != null)
                {
                    categoryInfo.RemoveRuleSetInfo(ruleSetDef);
                
                    foreach (var cat in Categories)
                    {
                        if (cat.RuleSets != null)
                        {
                            var ruleSetInfo = (from r in cat.RuleSets
                                               where r.RuleSetDef == ruleSetDef
                                               select r).FirstOrDefault();

                            if (ruleSetInfo != null)
                            {
                                return;
                            }
                        }
                    }
                    AddUncategorizedRuleSet(ruleSetDef);
                }
            }
            
            AddUncategorizedRuleSets(def.ThisRuleApp);
        }

        public void AddRuleSet(RuleSetDefBase ruleSetDefBase)
        {
            var ruleSetDef = ruleSetDefBase as RuleSetDef;
            if (ruleSetDef != null)
            {
                var uncatInfo = GetCategoryInfo(UNCATEGORIZED_CATEGORY, ruleSetDef.ThisRuleApp);
                if (uncatInfo != null)
                {
                    uncatInfo.RuleSets.Add(new RuleSetInfo(ruleSetDef));    
                }
            }
        }

        private void AddUncategorizedRuleSets(RuleApplicationDef ruleAppDef)
        {
            var ruleSets = (from r in ruleAppDef.AsEnumerable()
                            where r is RuleSetDef && r.AssignedCategories.Count == 0
                            select r).ToList<RuleSetDef>();

            var categoryInfo = GetCategoryInfo(UNCATEGORIZED_CATEGORY, ruleAppDef);
            
            if (ruleSets.Count > 0)
            {
                
                foreach (var ruleSetDef in ruleSets)
                {
                    var alreadyInList = (from r in categoryInfo.RuleSets
                                         where r.RuleSetDef == ruleSetDef
                                         select r).Count() == 1;

                    if (!alreadyInList)
                    {
                        categoryInfo.RuleSets.Add(new RuleSetInfo(ruleSetDef));
                    }
                }
            }
        }

        public void AddCategory(CategoryDef categoryDef)
        {
            Categories.Add(new CategoryInfo(categoryDef, categoryDef.ThisRuleApp));
        }

        public void RemoveCategory(CategoryDef categoryDef)
        {
            var categoryInfo = GetCategoryInfo(categoryDef.Name, categoryDef.ThisRuleApp);
            categoryInfo.RuleSets.Clear();
            Categories.Remove(categoryInfo);

            AddUncategorizedRuleSets(categoryDef.ThisRuleApp);
        }

        public void RemoveRuleSet(RuleSetDef ruleSetDef)
        {
            foreach (var categoryInfo in Categories)
            {
                categoryInfo.RemoveRuleSetInfo(ruleSetDef);
            }
        }

        private void AddUncategorizedRuleSet(RuleSetDef ruleSetDef)
        {
            var categoryInfo = GetCategoryInfo(UNCATEGORIZED_CATEGORY, ruleSetDef.ThisRuleApp);
            categoryInfo.RuleSets.Add(new RuleSetInfo(ruleSetDef));
        }
        private CategoryInfo GetCategoryInfo(string name, RuleApplicationDef ruleApplicationDef)
        {
            return GetCategoryInfo(name, ruleApplicationDef, true);
        }
        private CategoryInfo GetCategoryInfo(string name, RuleApplicationDef ruleApplicationDef, bool createIfDoesNotExist)
        {
            var categoryInfo = (from c in Categories
                               where c.CategoryDef.Name == name
                               select c).FirstOrDefault();

            if (categoryInfo != null)
            {
                return categoryInfo;
            }
            else
            {
                CategoryDef categoryDef = null;
                if (createIfDoesNotExist)
                {
                    categoryDef = (from c in ruleApplicationDef.Categories
                                       where c.Name == name
                                       select c).FirstOrDefault();
                
                    if (categoryDef == null && name == UNCATEGORIZED_CATEGORY)
                    {
                        categoryDef = new CategoryDef(UNCATEGORIZED_CATEGORY);
                    }

                    if (categoryDef != null)
                    {
                        categoryInfo = new CategoryInfo(categoryDef, ruleApplicationDef);

                        Categories.Add(categoryInfo);    
                    }
                    
                }
                return categoryInfo;
            }
        }
    }

    public class CategoryInfo
    {
        public CategoryInfo(CategoryDef categoryDef, RuleApplicationDef ruleAppDef)
        {
            if (categoryDef != null)
            {
                CategoryDef = categoryDef;
                var ruleSets = (from r in ruleAppDef.AsEnumerable()
                                where r is RuleSetDef && r.AssignedCategories.Contains(categoryDef.Name)
                                select r).ToList<RuleSetDef>();

                RuleSets = new ObservableCollection<RuleSetInfo>();

                ruleSets.ForEach(r => RuleSets.Add(new RuleSetInfo(r)));
            }
        }

        public void RemoveRuleSetInfo(RuleSetDef ruleSetDef)
        {
            if (ruleSetDef != null)
            {
                RuleSetInfo ruleSetInfo = null;

                if (RuleSets != null)
                {
                    foreach (var r in RuleSets)
                    {
                        if (r.RuleSetDef == ruleSetDef)
                        {
                            ruleSetInfo = r;
                        }
                    }

                    if (ruleSetInfo != null)
                    {
                        RuleSets.Remove(ruleSetInfo);
                    }    
                }
            }
        }

        public CategoryDef CategoryDef { get; set; }
        public ObservableCollection<RuleSetInfo> RuleSets { get; set; }
    }

    public class RuleSetInfo
    {
        public RuleSetInfo(RuleSetDef ruleSetDef)
        {
            RuleSetDef = ruleSetDef;
            EntityDef = ruleSetDef.ThisEntity;
        }

        public RuleSetDef RuleSetDef { get; set; }
        public EntityDef EntityDef { get; set; }
    }
}
