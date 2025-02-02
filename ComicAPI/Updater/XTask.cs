using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComicApp.Data;

namespace ComicAPI.Updater
{
    public class XTask
    {
        private ulong  _second = 1;
        private event Action? OnTrigger;

        public XTask()
        {
            _second = 1;
        }
        public void Register(Action action)
        {
            OnTrigger += action;
        }

        public XTask(ulong second)
        {
            _second = second;
        }

        public void Update(ulong curTick)
        {
            if((curTick % _second) == 0)
            {
                OnTrigger?.Invoke();
            }
        }
    }
    
    
}