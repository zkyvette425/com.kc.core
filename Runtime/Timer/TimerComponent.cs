using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace KC
{
    public class TimerComponent : Component, IAwake, IDestroy
    {
        private Dictionary<long, CancellationTokenSource> _tasks;

        public void Awake()
        {
            _tasks = new Dictionary<long, CancellationTokenSource>();
        }

        public void Destroy()
        {
            foreach (var task in _tasks.Values)
            {
                task.Cancel();
                task.Dispose();
            }

            _tasks.Clear();
        }

        public long Repeat(float milliseconds, Action<int> onComplete, bool ignoreTimeScale = false,
            PlayerLoopTiming delayTiming = PlayerLoopTiming.Update,
            bool cancelImmediately = false)
        {
            TimeSpan span = TimeSpan.FromMilliseconds(milliseconds);
            return Repeat(span, onComplete, ignoreTimeScale, delayTiming, cancelImmediately);
        }

        public long Repeat(TimeSpan repeatTime, Action<int> onComplete, bool ignoreTimeScale = false,
            PlayerLoopTiming delayTiming = PlayerLoopTiming.Update,
            bool cancelImmediately = false)
        {
            async UniTaskVoid CreateTask(CancellationToken token)
            {
                int runCount = 0;
                while (!token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                    await UniTask.Delay(repeatTime, ignoreTimeScale, delayTiming, token, cancelImmediately);
                    onComplete?.Invoke(++runCount);
                }
            }

            CancellationTokenSource source = new CancellationTokenSource();
            UniTask.Void(CreateTask, source.Token);
            var id = IdGenerator.Instance.GenerateId();
            _tasks.Add(id, source);
            return id;
        }

        public long RepeatOneFrame(Action<int> omComplete, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update,
            bool cancelImmediately = false)
        {
            return RepeatFrame(1, omComplete, delayTiming, cancelImmediately);
        }

        public long RepeatFrame(int delayFrameCount, Action<int> onComplete,
            PlayerLoopTiming delayTiming = PlayerLoopTiming.Update,
            bool cancelImmediately = false)
        {
            async UniTaskVoid CreateTask(CancellationToken token)
            {
                int runCount = 0;
                while (!token.IsCancellationRequested)
                {
                    token.ThrowIfCancellationRequested();
                    await UniTask.DelayFrame(delayFrameCount, delayTiming, token, cancelImmediately);
                    onComplete?.Invoke(++runCount);
                }
            }

            CancellationTokenSource source = new CancellationTokenSource();
            UniTask.Void(CreateTask, source.Token);
            var id = IdGenerator.Instance.GenerateId();
            _tasks.Add(id, source);
            return id;
        }

        public void Remove(long id)
        {
            if (_tasks.Remove(id, out var tokenSource))
            {
                tokenSource.Cancel();
                tokenSource.Dispose();
            }
        }
    }
}