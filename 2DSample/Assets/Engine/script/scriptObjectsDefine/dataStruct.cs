using UnityEngine;
using System.Reflection;
using System;
using System.Collections.Generic;

namespace BaseData
{
    /// <summary>
    /// 数据节点的核心，用于通过反射获取和存储一个对象的所有公共属性。
    /// 这是框架实现通用数据访问的基础。
    /// </summary>
    public class DataNode
    {
        // 存储属性信息
        private PropertyInfo[] infos;
        // 存储属性对应的实际数据
        private object[] datas;

        /// <summary>
        /// 构造函数，传入一个对象，通过反射获取其所有公共属性。
        /// </summary>
        /// <param name="o">要提取数据的对象。</param>
        public DataNode(object o)
        {
            // 注意：只有用 { get; set; } 封装过的 public 属性才会被 GetProperties() 识别到。
            infos = o.GetType().GetProperties();
            datas = new object[infos.Length];
            for (int i = 0; i < datas.Length; i++)
            {
                datas[i] = infos[i].GetValue(o);
            }
        }

        /// <summary>
        /// 根据属性名获取属性的类型。
        /// </summary>
        public Type getType(string name)
        {
            foreach (var i in infos)
            {
                if (i.Name == name)
                    return i.GetType();
            }
            return null;
        }

        /// <summary>
        /// 根据属性名获取数据。
        /// </summary>
        public object getData(string name)
        {
            for (int i = 0; i < infos.Length; i++)
            {
                if (infos[i].Name == name)
                    return datas[i];
            }
            return null;
        }

        /// <summary>
        /// 根据属性名获取指定类型的数据（泛型版本）。
        /// </summary>
        public T getData<T>(string name)
        {
            return (T)getData(name);
        }
    }

    /// <summary>
    /// 实现了 dataNodeInterface 接口的基类，封装了一个 DataNode 实例。
    /// </summary>
    public class DataBaseNode : dataNodeInterface
    {
        protected DataNode node = null;

        public Type getType(string name)
        {
            return node.getType(name);
        }

        public object getData(string name)
        {
            return node.getData(name);
        }

        public T getData<T>(string name)
        {
            return node.getData<T>(name);
        }
    }

    /// <summary>
    /// 列表节点的基类，在 DataBaseNode 的基础上增加了 id 属性。
    /// </summary>
    public class ListNode : DataBaseNode, ListNodeInterface
    {
        public string id;

        public string getId()
        {
            return id;
        }

        public int getNodeType()
        {
            // 从反射获取的数据中查找类型信息
            return (int)node.getData(propertyName.type);
        }
    }

    /// <summary>
    /// 树节点的基类，在 ListNode 的基础上增加了父子关系。
    /// 主要用于事件系统。
    /// </summary>
    public class TreeNode : ListNode, TreeNodeInterface
    {
        // 子节点列表
        protected List<TreeNodeInterface> children = new List<TreeNodeInterface>();
        // 父节点列表
        protected List<TreeNodeInterface> parents = new List<TreeNodeInterface>();

        public TreeNodeInterface getChild(string id)
        {
            foreach(var child in children)
                if(child.getId()==id)
                    return child;

            return null;
        }

        public void addChild(TreeNodeInterface c)
        {
            if (!children.Contains(c))
            {
                children.Add(c);
            }
        }

        public bool removeChild(TreeNodeInterface c)
        {
            if (children.Contains(c))
            {
                return children.Remove(c);
            }

            return true;
        }

        public List<TreeNodeInterface> getChildren()
        {
            return children;
        }



        public TreeNodeInterface getParent(string id)
        {
            foreach (var parent in parents)
                if (parent.getId() == id)
                    return parent;

            return null;
        }

        public void addParent(TreeNodeInterface c)
        {
            if (!parents.Contains(c))
            {
                parents.Add(c);
            }
        }

        public bool removeParent(TreeNodeInterface c)
        {
            if (parents.Contains(c))
            {
                return parents.Remove(c);
            }

            return true;
        }

        public List<TreeNodeInterface> getParents()
        {
            return parents;
        }
    }
}
