using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.InputSystem;
using UnityEngine.ResourceManagement.AsyncOperations;
using Object = UnityEngine.Object;

namespace HA
{
    /// <summary>
    /// 资源加载枚举状态
    /// </summary>
    public enum LoadResourceTaskState
    {
        Normal,
        Waiting,
        Loading,
        ToCancel,
        ToRelease
    }

    /// <summary>
    /// 资源加载任务
    /// </summary>
    public class LoadResourceTask
    {
        public string _key;
        public string _tag;
        public string _type;
        public int _refCount;
        public object[] _args;
        public LoadResourceTaskState _state;
        public Action<Object, object[]> _callback;
        public AsyncOperationHandle _handle;
    }

    /// <summary>
    /// 以 _tag 为标识的资源加载任务组
    /// </summary>
    public class LoadResourceTaskGroup
    {
        public string _tag;
        public List<LoadResourceTask> _tasks = new List<LoadResourceTask>();
    }

    public class ResourceComponent : BaseComponent
    {
        private List<LoadResourceTaskGroup> _taskGroups = new List<LoadResourceTaskGroup>();
        private List<LoadResourceTask> _completedTaskCache = new List<LoadResourceTask>();
        private bool _isLogEnable = false;

        private void LateUpdate()
        {
            FrameByFrameLoad();
        }

        /// <summary>
        /// 分帧加载本地资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">资源名称</param>
        /// <param name="tag">资源标签, 调用 Release 时使用</param>
        /// <param name="callback">回调</param>
        /// <param name="args">透传参数</param>
        public void LoadResourceAsync<T>(string key, string tag, Action<Object, object[]> callback, params object[] args)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(tag))
            {
                HADebug.LogError("加载资源的 [key] 或 [tag] 为空!");
                return;
            }

            if (_isLogEnable)
            {
                HADebug.LogFormat("分帧资源加载任务开始: key[{0}], tag[{1}]", key, tag);
            }

            // 查找缓存中是否存在该资源
            // 存在则返回该资源
            foreach (LoadResourceTask item in _completedTaskCache)
            {
                if (item._tag == tag && item._key == key)
                {
                    if (_isLogEnable)
                    {
                        HADebug.LogFormat("从缓存中加载资源成功: key[{0}], tag[{1}]", key, tag);
                    }
                    item._refCount++;
                    callback(item._handle.Result as Object, args);
                    return;
                }
            }

            // 缓存中不存在, 创建任务
            LoadResourceTask task = new LoadResourceTask
            {
                _key      = key,
                _tag      = tag,
                _type     = typeof(T).ToString(),
                _refCount = 1,
                _args     = args,
                _callback = callback
            };

            // 查找相同 _tag 的任务组
            foreach (LoadResourceTaskGroup group in _taskGroups)
            {
                foreach (LoadResourceTask t in group._tasks)
                {
                    // 如果存在相同的资源加载任务
                    if (t._tag.Equals(tag) &&
                        t._key.Equals(key) && 
                        t._state != LoadResourceTaskState.ToRelease &&
                        t._state != LoadResourceTaskState.ToCancel)
                    {
                        task._state = LoadResourceTaskState.Waiting;
                        break;
                    }
                }
            }

            // 查找相同 _tag 的任务组, 将任务加入其中
            bool isFind = false;
            foreach (LoadResourceTaskGroup group in _taskGroups)
            {
                if (group._tag == task._tag)
                {
                    group._tasks.Add(task);
                    isFind = true;
                    break;
                }
            }

            // 没有 _tag 对应的任务组, 则创建新任务组
            if (!isFind)
            {
                LoadResourceTaskGroup group = new LoadResourceTaskGroup();
                group._tag = task._tag;
                group._tasks.Add(task);
                _taskGroups.Add(group);
            }
        }

        /// <summary>
        /// 不分帧加载本地资源
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key">资源名称</param>
        /// <param name="tag">资源标签, 调用 Release 时使用</param>
        /// <returns></returns>
        public async Task<T> LoadAsset<T>(string key, string tag)
        {
            if (string.IsNullOrEmpty(key) || string.IsNullOrEmpty(tag))
            {
                HADebug.LogError("加载资源的 [key] 或 [tag] 为空!");
                return default;
            }

            if (_isLogEnable)
            {
                HADebug.LogFormat("分帧资源加载任务开始: key[{0}], tag[{1}]", key, tag);
            }

            // 查找缓存中是否存在该资源
            // 存在则返回该资源
            foreach (LoadResourceTask item in _completedTaskCache)
            {
                if (item._tag == tag && item._key == key)
                {
                    if (_isLogEnable)
                    {
                        HADebug.LogFormat("从缓存中加载资源成功: key[{0}], tag[{1}]", key, tag);
                    }
                    item._refCount++;
                    return (T)item._handle.Result;
                }
            }

            AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(key);
            var result = await handle.Task;

            LoadResourceTask task = new LoadResourceTask
            {
                _key = key,
                _tag = tag,
                _refCount = 1,
                _handle = handle
            };

            _completedTaskCache.Add(task);

            if (_isLogEnable)
            {
                HADebug.LogFormat("加载资源成功: key[{0}], tag[{1}]", key, tag);
            }

            return result;
        }

        /// <summary>
        /// 取消加载
        /// </summary>
        /// <param name="tag">资源标签</param>
        public void Cancel(string tag)
        {
            foreach (LoadResourceTaskGroup group in _taskGroups)
            {
                foreach (LoadResourceTask task in group._tasks)
                {
                    if (task._tag == tag)
                    {
                        task._state = LoadResourceTaskState.ToCancel;
                    }
                }
            }
        }

        /// <summary>
        /// 在 LateUpdate 中每一帧从句柄列表中释放一个资源 (分帧)
        /// </summary>
        /// <param name="tag">资源标签</param>
        public void Release(string tag)
        {
            if (_isLogEnable)
            {
                HADebug.LogFormat("将要释放资源: tag[{0}]", tag);
            }

            foreach (LoadResourceTaskGroup group in _taskGroups)
            {
                foreach (LoadResourceTask task in group._tasks)
                {
                    if (task._tag == tag)
                    {
                        task._refCount--;
                        if (task._refCount <= 0)
                        {
                            task._state = LoadResourceTaskState.ToRelease;
                        }
                    }
                }
            }

            foreach (LoadResourceTask task in _completedTaskCache)
            {
                if (task._tag == tag)
                {
                    task._refCount--;
                    if (task._refCount <= 0)
                    {
                        task._state = LoadResourceTaskState.ToRelease;
                    }
                }
            }
        }

        /// <summary>
        /// 分帧加载资源
        /// </summary>
        private void FrameByFrameLoad()
        {
            bool isDone = false;

            // 选择一个待加载任务
            // 倒序, 保证后进资源优先加载
            for (int i = _taskGroups.Count - 1; i >= 0; i--)
            {
                LoadResourceTaskGroup group = _taskGroups[i];

                // 任务组为空
                if (group._tasks.Count == 0)
                {
                    _taskGroups.Remove(group);
                    continue;
                }

                // 任务组不为空
                for (int j = 0; j < group._tasks.Count; j++)
                {
                    LoadResourceTask task = group._tasks[j];

                    // 如果是将要释放或取消的任务
                    if (task._state == LoadResourceTaskState.ToRelease ||
                        task._state == LoadResourceTaskState.ToCancel)
                    {
                        j--;
                        group._tasks.Remove(task);
                        if (_isLogEnable)
                        {
                            HADebug.LogFormat("分帧资源加载已取消: key[{0}], tag[{1}]", task._key, task._tag);
                        }
                        continue;
                    }
                    else if (task._state == LoadResourceTaskState.Normal)
                    {
                        // 查找缓存中是否存在相同资源
                        foreach (LoadResourceTask t in _completedTaskCache)
                        {
                            if (t._tag == task._tag && t._key == task._key)
                            {
                                if (_isLogEnable)
                                {
                                    HADebug.LogFormat("从缓存中加载资源成功: key[{0}], tag[{1}]", t._key, t._tag);
                                }
                                t._refCount++;
                                task._callback(t._handle.Result as Object, task._args);
                                group._tasks.Remove(task);
                                break;
                            }
                        }

                        isDone = true;

                        if (task._type == "UnityEngine.Sprite")
                        {
                            task._handle = Addressables.LoadAssetAsync<UnityEngine.Sprite>(task._key);
                            task._state = LoadResourceTaskState.Loading;
                        }
                        else
                        {
                            task._handle = Addressables.LoadAssetAsync<UnityEngine.Object>(task._key);
                            task._state = LoadResourceTaskState.Loading;
                        }
                        break;
                    }
                }

                if (isDone) break;
            }

            // 检测加载是否完成
            for (int i = 0; i < _taskGroups.Count; i++)
            {
                LoadResourceTaskGroup group = _taskGroups[i];

                if (group._tasks.Count == 0)
                {
                    i--;
                    _taskGroups.Remove(group);
                    continue;
                }

                for (int j = 0; j < group._tasks.Count; j++)
                {
                    LoadResourceTask task = group._tasks[j];
                    if (task._state == LoadResourceTaskState.Loading)
                    {
                        if (task._handle.IsValid())
                        {
                            if (task._handle.Status == AsyncOperationStatus.Succeeded)
                            {
                                if (_isLogEnable)
                                {
                                    HADebug.LogFormat("分帧加载资源成功: key[{0}], tag[{1}]", task._key, task._tag);
                                }
                                if (task._type == "UnityEngine.Sprite")
                                {
                                    task._callback((UnityEngine.Sprite)task._handle.Result, task._args);
                                }
                                else
                                {
                                    task._callback((UnityEngine.Object)task._handle.Result, task._args);
                                }


                                group._tasks.Remove(task);
                                _completedTaskCache.Add(task);

                                // 将具有相同 key 值的资源状态修改为 Normal
                                // 从而使其可以在下一帧触发从缓存加载
                                foreach (LoadResourceTaskGroup g in _taskGroups)
                                {
                                    foreach (LoadResourceTask t in g._tasks)
                                    {
                                        if (t._key.Equals(task._key) && t._state == LoadResourceTaskState.Waiting)
                                        {
                                            t._state = LoadResourceTaskState.Normal;
                                        }
                                    }
                                }

                                j--;
                            }
                            else if (task._handle.Status == AsyncOperationStatus.Failed)
                            {
                                HADebug.LogErrorFormat("需要加载的资源不存在: key[{0}]", task._key);
                                j--;
                                task._callback(null, task._args);
                                group._tasks.Remove(task);
                            }
                        }
                    }
                }
            }

            // 资源释放检测
            for (int i = 0; i < _completedTaskCache.Count; i++)
            {
                LoadResourceTask task = _completedTaskCache[i];

                if (task._state == LoadResourceTaskState.ToRelease &&
                    task._handle.IsValid())
                {
                    i--;
                    Addressables.Release(task._handle.Result);
                    _completedTaskCache.Remove(task);
                    if (_isLogEnable)
                    {
                        HADebug.LogFormat("释放资源成功: key[{0}], tag[{1}]", task._key, task._tag);
                    }
                }
            }
        }
    }
}
