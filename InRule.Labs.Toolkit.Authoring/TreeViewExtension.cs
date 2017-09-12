using System.Windows.Controls;
using InRule.Labs.Toolkit.Shared.Model;

namespace InRule.Labs.Toolkit.Authoring
{
    public static class TreeViewExtension
    {

        public static TreeViewItem ContainerFromItem(this TreeView treeView, object item)

        {

            TreeViewItem containerThatMightContainItem = (TreeViewItem)treeView.ItemContainerGenerator.ContainerFromItem(item);

            if (containerThatMightContainItem != null)

                return containerThatMightContainItem;

            else

                return ContainerFromItem(treeView.ItemContainerGenerator, treeView.Items, item);

        }


        private static TreeViewItem ContainerFromItem(ItemContainerGenerator parentItemContainerGenerator, ItemCollection itemCollection, object item)

        {

            foreach (object curChildItem in itemCollection)

            {

                TreeViewItem parentContainer = (TreeViewItem)parentItemContainerGenerator.ContainerFromItem(curChildItem);
                TreeViewItem containerThatMightContainItem = null;
                if (parentContainer != null)
                {
                    containerThatMightContainItem = (TreeViewItem)parentContainer.ItemContainerGenerator.ContainerFromItem(item);
                }
                

                if (containerThatMightContainItem != null)

                    return containerThatMightContainItem;
                TreeViewItem recursionResult = null;
                if (parentContainer != null)
                {
                    recursionResult = ContainerFromItem(parentContainer.ItemContainerGenerator, parentContainer.Items, item);
                }
                 

                if (recursionResult != null)

                    return recursionResult;

            }

            return null;

        }


        public static object ItemFromContainer(this TreeView treeView, TreeViewItem container)

        {

            TreeViewItem itemThatMightBelongToContainer = (TreeViewItem)treeView.ItemContainerGenerator.ItemFromContainer(container);

            if (itemThatMightBelongToContainer != null)

                return itemThatMightBelongToContainer;

            else

                return ItemFromContainer(treeView.ItemContainerGenerator, treeView.Items, container);

        }


        private static object ItemFromContainer(ItemContainerGenerator parentItemContainerGenerator,
            ItemCollection itemCollection, TreeViewItem container)

        {

            foreach (object curChildItem in itemCollection)

            {

                TreeViewItem parentContainer =
                    (TreeViewItem) parentItemContainerGenerator.ContainerFromItem(curChildItem);

                TreeViewItem itemThatMightBelongToContainer =
                    (TreeViewItem) parentContainer.ItemContainerGenerator.ItemFromContainer(container);

                if (itemThatMightBelongToContainer != null)

                    return itemThatMightBelongToContainer;

                TreeViewItem recursionResult =
                    ItemFromContainer(parentContainer.ItemContainerGenerator, parentContainer.Items,
                        container) as TreeViewItem;

                if (recursionResult != null)

                    return recursionResult;

            }

            return null;

        }


    }
}