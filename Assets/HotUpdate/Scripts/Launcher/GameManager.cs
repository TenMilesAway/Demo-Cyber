using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class GameManager : MonoBehaviour
    {
        private static readonly List<BaseComponent> s_Components = new List<BaseComponent>();

        public static DelayedTaskComponent DelayedTask
        {
            get;
            private set;
        }

        public static DataTableComponent DataTable
        {
            get;
            private set;
        }

        private void Start()
        {
            InitComponents();
        }

        /// <summary>
        /// 注册组件
        /// </summary>
        /// <param name="component"></param>
        public static void RegisterComponent(BaseComponent component)
        {
            if (component == null)
            {
                HADebug.LogError("Game Manager's component is invalid");
                return;
            }

            Type type = component.GetType();

            foreach (BaseComponent current in s_Components)
            {
                if (current != null && current.GetType() == type)
                {
                    HADebug.LogErrorFormat("Game Mananger's component type '{0}' is already exist.", type.FullName);
                    return;
                }
            }

            s_Components.Add(component);
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        private static void InitComponents()
        {
            DelayedTask = GetTargetComponent<DelayedTaskComponent>();
            DataTable = GetTargetComponent<DataTableComponent>();
        }

        /// <summary>
        /// 获得指定类型的 Component
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        private static T GetTargetComponent<T>() where T : BaseComponent
        {
            return (T)GetTargetComponent(typeof(T));
        }

        private static BaseComponent GetTargetComponent(Type type)
        {
            foreach (BaseComponent current in s_Components)
            {
                if (current != null && current.GetType() == type)
                {
                    return current;
                }
            }

            return null;
        }
    }
}
