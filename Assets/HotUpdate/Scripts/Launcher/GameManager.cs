using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HA
{
    public class GameManager : MonoBehaviour
    {
        private static readonly List<BaseComponent> s_Components = new List<BaseComponent>();

        /// <summary>
        /// 网络
        /// </summary>
        public static NetworkComponent Network { get; private set; }

        /// <summary>
        /// 事件
        /// </summary>
        public static EventComponent Event { get; private set; }

        /// <summary>
        /// 时间
        /// </summary>
        public static TimerComponent Timer { get; private set; }

        /// <summary>
        /// 延迟任务
        /// </summary>
        public static DelayedTaskComponent DelayedTask { get; private set; }

        /// <summary>
        /// 配置表
        /// </summary>
        public static DataTableComponent DataTable { get; private set; }

        /// <summary>
        /// 控制台输出
        /// </summary>
        public static ConsoleComponent Console { get; private set; }

        /// <summary>
        /// 资源加载
        /// </summary>
        public static ResourceComponent Resource { get; private set; }

        /// <summary>
        /// 音频(未完成)
        /// </summary>
        public static AudioComponent Audio { get; private set; }

        private void Start()
        {
            InitComponents();
        }

        /// <summary>
        /// 注册组件
        /// </summary>
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
            Network     = GetTargetComponent<NetworkComponent>();
            Event       = GetTargetComponent<EventComponent>();
            Timer       = GetTargetComponent<TimerComponent>();
            DelayedTask = GetTargetComponent<DelayedTaskComponent>();
            DataTable   = GetTargetComponent<DataTableComponent>();
            Console     = GetTargetComponent<ConsoleComponent>();
            Resource    = GetTargetComponent<ResourceComponent>();
            Audio       = GetTargetComponent<AudioComponent>();
        }

        /// <summary>
        /// 获得指定类型的 Component
        /// </summary>
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
